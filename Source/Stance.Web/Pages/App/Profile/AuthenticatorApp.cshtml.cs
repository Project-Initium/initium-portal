// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.DrawingCore.Imaging;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using QRCoder;
using Stance.Core;
using Stance.Core.Contracts;
using Stance.Core.Settings;
using Stance.Domain.Commands.UserAggregate;
using Stance.Queries.Contracts.Static;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.App.Profile
{
    public class AuthenticatorApp : PrgPageModel<AuthenticatorApp.Model>
    {
        [SuppressMessage("Minor Code Smell", "S1075:URIs should not be hardcoded",
            Justification = "This is the fix template used by totp apps")]
        private const string AuthenticatorUriFormat = "otpauth://totp/{0}:{1}?secret={2}&issuer={0}&digits=6";

        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IMediator _mediator;
        private readonly SecuritySettings _securitySettings;
        private readonly UrlEncoder _urlEncoder;

        private readonly IUserQueries _userQueries;

        public AuthenticatorApp(IUserQueries userQueries, IMediator mediator, UrlEncoder urlEncoder,
            IOptions<SecuritySettings> securitySettings,
            ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            if (securitySettings == null)
            {
                throw new ArgumentNullException(nameof(securitySettings));
            }

            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ??
                                                     throw new ArgumentNullException(
                                                         nameof(currentAuthenticatedUserProvider));
            this._userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._urlEncoder = urlEncoder ?? throw new ArgumentNullException(nameof(urlEncoder));
            this._securitySettings = securitySettings.Value;
        }

        public string Code { get; set; }

        public string AuthenticatorUri { get; set; }

        public string SharedKey { get; set; }

        public bool IsSetup { get; set; }

        public async Task OnGetAsync()
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                this.PrgState = PrgState.InError;
                return;
            }

            if (currentUserMaybe.Value is AuthenticatedUser user)
            {
                var userCheck = await this._userQueries.CheckForPresenceOfAuthAppForCurrentUser();

                if (userCheck.IsPresent)
                {
                    this.IsSetup = true;
                }
                else
                {
                    if (this.PageModel == null)
                    {
                        var result = await this._mediator.Send(new InitiateAuthenticatorAppEnrollmentCommand());
                        if (result.IsFailure)
                        {
                            this.PrgState = PrgState.InError;
                            return;
                        }

                        this.PageModel = new Model
                        {
                            SharedKey = result.Value.SharedKey,
                        };
                    }

                    this.SharedKey = FormatAuthenticatorAppKey(this.PageModel.SharedKey);
                    this.AuthenticatorUri = string.Format(
                        AuthenticatorUriFormat,
                        this._urlEncoder.Encode(this._securitySettings.SiteName),
                        this._urlEncoder.Encode(user.EmailAddress),
                        this.PageModel.SharedKey);

                    var qrGenerator = new QRCodeGenerator();

                    var qrCodeData = qrGenerator.CreateQrCode(this.AuthenticatorUri, QRCodeGenerator.ECCLevel.Q);

                    var qrCode = new QRCode(qrCodeData);
                    var qrCodeImage = qrCode.GetGraphic(20);

                    await using var stream = new MemoryStream();
                    qrCodeImage.Save(stream, ImageFormat.Png);
                    var bytes = stream.ToArray();
                    this.Code = Convert.ToBase64String(bytes);
                }
            }
            else
            {
                this.PrgState = PrgState.InError;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result =
                await this._mediator.Send(
                    new EnrollAuthenticatorAppCommand(this.PageModel.SharedKey, this.PageModel.Code));
            if (result.IsSuccess)
            {
                this.PrgState = PrgState.Success;
                this.AddPageNotification("Authenticator App", "Your app has been setup.", PageNotification.Success);
            }
            else
            {
                this.AddPageNotification("Authenticator App", "There has been an issue setting up your app.", PageNotification.Error);
                this.PrgState = PrgState.Failed;
            }

            return this.RedirectToPage();
        }

        public async Task<IActionResult> OnPostRevokeAsync()
        {
            var result = await this._mediator.Send(new RevokeAuthenticatorAppCommand());
            if (result.IsSuccess)
            {
                this.PrgState = PrgState.Success;
                this.AddPageNotification("Authenticator App", "Your app has been revoked.", PageNotification.Success);
            }
            else
            {
                this.AddPageNotification("Authenticator App", "There has been an issue revoking up your app.", PageNotification.Error);
                this.PrgState = PrgState.Failed;
            }

            return this.RedirectToPage();
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

        public class Model
        {
            public string Code { get; set; }

            public string SharedKey { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.SharedKey)
                    .NotEmpty();
                this.RuleFor(x => x.Code)
                    .NotEmpty().WithMessage("Please enter the code from your authenticator app.");
            }
        }
    }
}