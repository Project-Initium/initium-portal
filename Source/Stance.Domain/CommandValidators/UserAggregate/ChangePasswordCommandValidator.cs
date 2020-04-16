using System.Collections.Generic;
using System.Text;
using FluentValidation;
using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandValidators.UserAggregate
{
    public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator()
        {
            this.RuleFor(x => x.CurrentPassword)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
            this.RuleFor(x => x.NewPassword)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired);
        }
    }
}
