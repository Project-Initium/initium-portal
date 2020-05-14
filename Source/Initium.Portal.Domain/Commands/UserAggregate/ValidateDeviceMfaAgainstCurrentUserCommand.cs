// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.UserAggregate
{
    public class
        ValidateDeviceMfaAgainstCurrentUserCommand : IRequest<
            Result<ValidateDeviceMfaAgainstCurrentUserCommandResult, ErrorData>>
    {
        public ValidateDeviceMfaAgainstCurrentUserCommand(
            AuthenticatorAssertionRawResponse authenticatorAssertionRawResponse, AssertionOptions assertionOptions)
        {
            this.AuthenticatorAssertionRawResponse = authenticatorAssertionRawResponse;
            this.AssertionOptions = assertionOptions;
        }

        public AuthenticatorAssertionRawResponse AuthenticatorAssertionRawResponse { get; }

        public AssertionOptions AssertionOptions { get; }
    }
}