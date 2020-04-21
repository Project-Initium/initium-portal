// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation;
using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandValidators.UserAggregate
{
    public class
        ValidateDeviceMfaAgainstCurrentUserCommandValidator : AbstractValidator<ValidateDeviceMfaAgainstCurrentUserCommand>
    {
        public ValidateDeviceMfaAgainstCurrentUserCommandValidator()
        {
            this.RuleFor(x => x.AssertionOptions)
                .NotNull().WithErrorCode(ValidationCodes.FieldIsRequired);
            this.RuleFor(x => x.AuthenticatorAssertionRawResponse)
                .NotNull().WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}