// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Stance.Domain.CommandResults.UserAggregate
{
    public class ValidateEmailMfaCodeAgainstCurrentUserCommandResult : BaseAuthenticationProcessCommandResult
    {
        public ValidateEmailMfaCodeAgainstCurrentUserCommandResult(Guid userId, string emailAddress, string firstName,
            string lastName)
            : base(userId, emailAddress, firstName, lastName)
        {
        }

        public ValidateEmailMfaCodeAgainstCurrentUserCommandResult(
            Guid userId, AuthenticationState authenticationStatus)
            : base(userId, authenticationStatus)
        {
        }
    }
}