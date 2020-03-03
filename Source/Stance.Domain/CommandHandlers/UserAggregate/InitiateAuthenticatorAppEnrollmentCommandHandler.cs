// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OtpNet;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandResults.UserAggregate;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Domain.CommandHandlers.UserAggregate
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