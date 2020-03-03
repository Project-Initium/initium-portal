// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation;
using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandValidators.UserAggregate
{
    public class EnrollAuthenticatorAppCommandValidator : AbstractValidator<EnrollAuthenticatorAppCommand>
    {
        public EnrollAuthenticatorAppCommandValidator()
        {
            this.RuleFor(x => x.Key)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
            this.RuleFor(x => x.Code)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}