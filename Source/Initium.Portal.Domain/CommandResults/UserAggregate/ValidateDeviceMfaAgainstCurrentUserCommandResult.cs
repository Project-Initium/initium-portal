// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Fido2NetLib.Objects;

namespace Initium.Portal.Domain.CommandResults.UserAggregate
{
    public class ValidateDeviceMfaAgainstCurrentUserCommandResult : BaseAuthenticationProcessCommandResult
    {
        public ValidateDeviceMfaAgainstCurrentUserCommandResult(
            Guid userId, AuthenticationState authenticationStatus)
            : base(userId, authenticationStatus)
        {
        }

        public ValidateDeviceMfaAgainstCurrentUserCommandResult(
            Guid userId, AssertionVerificationResult assertionVerificationResult)
            : base(userId, AuthenticationState.Completed)
        {
            this.AssertionVerificationResult = assertionVerificationResult;
        }

        public AssertionVerificationResult AssertionVerificationResult { get; }
    }
}