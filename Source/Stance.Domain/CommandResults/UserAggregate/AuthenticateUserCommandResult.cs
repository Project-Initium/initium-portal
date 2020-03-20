// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Core.Constants;

namespace Stance.Domain.CommandResults.UserAggregate
{
    public class AuthenticateUserCommandResult : BaseAuthenticationProcessCommandResult
    {
        public AuthenticateUserCommandResult(Guid userId)
            : base(userId, AuthenticationState.Completed)
        {
        }

        public AuthenticateUserCommandResult(Guid userId, AuthenticationState authenticationStatus, MfaProvider mfaProviders)
            : base(userId, authenticationStatus)
        {
            this.SetupMfaProviders = mfaProviders;
        }

        public MfaProvider SetupMfaProviders { get; }
    }
}