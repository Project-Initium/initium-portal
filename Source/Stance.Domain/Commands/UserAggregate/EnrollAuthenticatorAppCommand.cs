using MediatR;
using ResultMonad;
using Stance.Core.Domain;

namespace Stance.Domain.Commands.UserAggregate
{
    public class EnrollAuthenticatorAppCommand : IRequest<ResultWithError<ErrorData>>
    {
        public EnrollAuthenticatorAppCommand(string key, string code)
        {
            this.Key = key;
            this.Code = code;
        }

        public string Key { get; }

        public string Code { get; }
    }
}