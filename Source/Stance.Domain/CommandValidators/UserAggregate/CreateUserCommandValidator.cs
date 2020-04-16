// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation;
using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandValidators.UserAggregate
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            this.RuleFor(x => x.EmailAddress)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired)
                .EmailAddress().WithErrorCode(ValidationCodes.ValueMustBeAnEmailAddress);
            this.RuleFor(x => x.FirstName)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
            this.RuleFor(x => x.LastName)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}