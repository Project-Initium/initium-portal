// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.DrawingCore.Imaging;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QRCoder;
using Stance.Core;
using Stance.Core.Contracts;
using Stance.Core.Settings;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Web.Controllers.Api.AuthApp
{
    [Authorize]
    public class AuthAppApiController : Controller
    {
        [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded",
            Justification = "This is the fix template used by totp apps")]
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        private readonly IMediator _mediator;
        private readonly SecuritySettings _securitySettings;
        private readonly UrlEncoder _urlEncoder;

        public AuthAppApiController(IMediator mediator, IOptions<SecuritySettings> securitySettings,
            UrlEncoder urlEncoder, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            if (securitySettings == null)
            {
                throw new ArgumentNullException(nameof(securitySettings));
            }

            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._securitySettings = securitySettings.Value;
            this._urlEncoder = urlEncoder ?? throw new ArgumentNullException(nameof(urlEncoder));
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ??
                                                     throw new ArgumentNullException(
                                                         nameof(currentAuthenticatedUserProvider));
        }

        [HttpPost("api/auth-app/initiate-enrollment")]
        public async Task<IActionResult> InitiateAuthAppEnrollment()
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                return this.Json(new InitiateAuthAppEnrollmentResponse());
            }

            if (!(currentUserMaybe.Value is AuthenticatedUser user))
            {
                return this.Json(new InitiateAuthAppEnrollmentResponse());
            }

            var result = await this._mediator.Send(new InitiateAuthenticatorAppEnrollmentCommand());
            if (result.IsFailure)
            {
                return this.Json(new InitiateAuthAppEnrollmentResponse());
            }

            var formattedSharedKey = FormatAuthenticatorAppKey(result.Value.SharedKey);
            var authenticatorUri = string.Format(
                AuthenticatorUriFormat,
                this._urlEncoder.Encode(this._securitySettings.SiteName),
                this._urlEncoder.Encode(user.EmailAddress),
                result.Value.SharedKey);
            return this.Json(new InitiateAuthAppEnrollmentResponse(result.Value.SharedKey, formattedSharedKey,
                authenticatorUri));
        }

        [HttpPost("api/auth-app/complete-enrollment")]
        public async Task<IActionResult> EnrollAuthApp([FromBody] EnrollAuthAppRequest model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Json(new EnrollAuthAppResponse(false));
            }

            var result =
                await this._mediator.Send(
                    new EnrollAuthenticatorAppCommand(model.SharedKey, model.Code));
            return this.Json(new EnrollAuthAppResponse(result.IsSuccess));
        }

        [HttpPost("api/auth-app/revoke")]
        public async Task<IActionResult> RevokeAuthApp([FromBody] RevokeAuthAppRequest model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Json(new RevokeAuthAppResponse(false));
            }

            var result = await this._mediator.Send(new RevokeAuthenticatorAppCommand(model.Password));
            return this.Json(new RevokeAuthAppResponse(result.IsSuccess));
        }

        [HttpGet("api/auth-app/mfa-qrcode.png")]
        public async Task<IActionResult> MfaQrCode([FromQuery] string authenticatorUri)
        {
            var qrGenerator = new QRCodeGenerator();

            var qrCodeData = qrGenerator.CreateQrCode(authenticatorUri, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            await using var stream = new MemoryStream();
            qrCodeImage.Save(stream, ImageFormat.Png);
            var bytes = stream.ToArray();
            return this.File(bytes, "image/png");
        }

        private static string FormatAuthenticatorAppKey(string base32Key)
        {
            var result = new StringBuilder();
            var currentPosition = 0;
            while (currentPosition + 4 < base32Key.Length)
            {
                result.Append(base32Key.Substring(currentPosition, 4)).Append(" ");
                currentPosition += 4;
            }

            if (currentPosition < base32Key.Length)
            {
                result.Append(base32Key.Substring(currentPosition));
            }

            return result.ToString().ToLowerInvariant();
        }
    }
}