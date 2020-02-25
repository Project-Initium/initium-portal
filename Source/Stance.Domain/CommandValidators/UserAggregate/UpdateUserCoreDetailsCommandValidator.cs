// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using FluentValidation;
using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandValidators.UserAggregate
{
    public class UpdateUserCoreDetailsCommandValidator : AbstractValidator<UpdateUserCoreDetailsCommand>
    {
        public UpdateUserCoreDetailsCommandValidator()
        {
            this.RuleFor(x => x.UserId)
                .NotEqual(Guid.Empty).WithErrorCode(ValidationCodes.FieldIsRequired);

            this.RuleFor(x => x.EmailAddress)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired)
                .EmailAddress().WithErrorCode(ValidationCodes.ValueMustBeAnEmailAddress);
        }
    }
}