// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Domain.CommandResults.UserAggregate
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