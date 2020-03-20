// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Stance.Domain.CommandResults.UserAggregate
{
    public class ValidateAppMfaCodeAgainstCurrentUserCommandResult : BaseAuthenticationProcessCommandResult
    {
        public ValidateAppMfaCodeAgainstCurrentUserCommandResult(Guid userId)
            : base(userId, AuthenticationState.Completed)
        {
        }

        public ValidateAppMfaCodeAgainstCurrentUserCommandResult(
            Guid userId, AuthenticationState authenticationStatus)
            : base(userId, authenticationStatus)
        {
        }
    }
}