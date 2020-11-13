// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;

namespace Initium.Portal.Domain.CommandValidators.UserAggregate
{
    public class CreateInitialUserCommandValidator : AbstractValidator<CreateInitialUserCommand>
    {
        public CreateInitialUserCommandValidator()
        {
            this.RuleFor(x => x.EmailAddress)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired)
                .EmailAddress().WithErrorCode(ValidationCodes.ValueMustBeAnEmailAddress);

            this.RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);

            this.RuleFor(x => x.FirstName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);

            this.RuleFor(x => x.LastName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}