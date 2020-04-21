// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;
using MediatR;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.CommandResults.UserAggregate;

namespace Stance.Domain.Commands.UserAggregate
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