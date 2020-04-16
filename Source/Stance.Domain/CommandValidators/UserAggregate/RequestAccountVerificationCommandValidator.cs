using FluentValidation;
using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandValidators.UserAggregate
{
    public class RequestAccountVerificationCommandValidator : AbstractValidator<RequestAccountVerificationCommand>
    {
        public RequestAccountVerificationCommandValidator()
        {
            this.RuleFor(x => x.EmailAddress)
                .NotEmpty().WithErrorCode(ValidationCodes.FieldIsRequired)
                .EmailAddress().WithErrorCode(ValidationCodes.ValueMustBeAnEmailAddress);
        }
    }
}