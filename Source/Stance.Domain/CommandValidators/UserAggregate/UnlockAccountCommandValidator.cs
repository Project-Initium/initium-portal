using System;
using FluentValidation;
using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandValidators.UserAggregate
{
    public class UnlockAccountCommandValidator : AbstractValidator<UnlockAccountCommand>
    {
        public UnlockAccountCommandValidator()
        {
            this.RuleFor(x => x.UserId)
                .NotEqual(Guid.Empty).WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}