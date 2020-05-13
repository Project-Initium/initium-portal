// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;
using FluentValidation;

namespace Initium.Portal.Web.Controllers.Api.AuthDevice.Models
{
    public class CompleteAuthDeviceRegistrationRequest
    {
        public AuthenticatorAttestationRawResponse AttestationResponse { get; set; }

        public string Name { get; set; }

        public class
            CompleteAuthDeviceRegistrationRequestValidator : AbstractValidator<CompleteAuthDeviceRegistrationRequest>
        {
            public CompleteAuthDeviceRegistrationRequestValidator()
            {
                this.RuleFor(x => x.Name)
                    .NotEmpty();
            }
        }
    }
}