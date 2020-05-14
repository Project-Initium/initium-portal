// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using OtpNet;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.UserAggregate
{
    public class InitiateAuthenticatorAppEnrollmentCommandHandler : IRequestHandler<
        InitiateAuthenticatorAppEnrollmentCommand, Result<InitiateAuthenticatorAppEnrollmentCommandResult, ErrorData>>
    {
        public Task<Result<InitiateAuthenticatorAppEnrollmentCommandResult, ErrorData>> Handle(
            InitiateAuthenticatorAppEnrollmentCommand request, CancellationToken cancellationToken)
        {
            var key = KeyGeneration.GenerateRandomKey();
            var keyAsBase32String = Base32Encoding.ToString(key);

            return Task.FromResult(Result.Ok<InitiateAuthenticatorAppEnrollmentCommandResult, ErrorData>(
                new InitiateAuthenticatorAppEnrollmentCommandResult(keyAsBase32String)));
        }
    }
}