using MediatR;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.CommandResults.UserAggregate;

namespace Stance.Domain.Commands.UserAggregate
{
    public class
        InitiateAuthenticatorAppEnrollmentCommand : IRequest<
            Result<InitiateAuthenticatorAppEnrollmentCommandResult, ErrorData>>
    {
    }
}