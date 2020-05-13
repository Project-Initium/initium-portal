// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;

namespace Initium.Portal.Domain.CommandValidators.UserAggregate
{
    public class EnrollAuthenticatorDeviceCommandValidator : AbstractValidator<EnrollAuthenticatorDeviceCommand>
    {
        public EnrollAuthenticatorDeviceCommandValidator()
        {
            this.RuleFor(x => x.Name)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
            this.RuleFor(x => x.AuthenticatorAttestationRawResponse)
                .NotNull().WithErrorCode(ValidationCodes.FieldIsRequired);
            this.RuleFor(x => x.CredentialCreateOptions)
                .NotNull().WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}