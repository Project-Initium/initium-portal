using FluentValidation;
using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandValidators.UserAggregate
{
    public class
        InitiateAuthenticatorDeviceEnrollmentCommandValidator : AbstractValidator<
            InitiateAuthenticatorDeviceEnrollmentCommand>
    {
    }
}