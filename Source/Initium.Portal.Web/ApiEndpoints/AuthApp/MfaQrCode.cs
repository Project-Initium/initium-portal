// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.DrawingCore.Imaging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using QRCoder;

namespace Initium.Portal.Web.ApiEndpoints.AuthApp
{
    public class MfaQrCode : BaseAsyncEndpoint<MfaQrCode.EndpointRequest, BasicEndpointResponse>
    {
        [HttpGet("api/auth-app/mfa-qrcode.png")]
        public override async Task<ActionResult<BasicEndpointResponse>> HandleAsync([FromQuery] EndpointRequest request, CancellationToken cancellationToken = default)
        {
            if (!this.ModelState.IsValid)
            {
                return this.NotFound();
            }

            var qrGenerator = new QRCodeGenerator();

            var qrCodeData = qrGenerator.CreateQrCode(request.AuthenticatorUri, QRCodeGenerator.ECCLevel.Q);

            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(20);

            await using var stream = new MemoryStream();
            qrCodeImage.Save(stream, ImageFormat.Png);
            var bytes = stream.ToArray();
            return this.File(bytes, "image/png");
        }

        public class EndpointRequest
        {
            public string AuthenticatorUri { get; set; }
        }

        public class EndpointRequestValidator : AbstractValidator<EndpointRequest>
        {
            public EndpointRequestValidator()
            {
                this.RuleFor(x => x.AuthenticatorUri)
                    .NotEmpty();
            }
        }
    }
}