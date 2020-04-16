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