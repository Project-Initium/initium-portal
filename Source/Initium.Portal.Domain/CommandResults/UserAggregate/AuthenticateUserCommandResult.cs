// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Fido2NetLib;
using Initium.Portal.Core.Constants;

namespace Initium.Portal.Domain.CommandResults.UserAggregate
{
    public class AuthenticateUserCommandResult : BaseAuthenticationProcessCommandResult
    {
        private readonly AssertionOptions _assertionOptions;

        public AuthenticateUserCommandResult(Guid userId, AuthenticationState authenticationStatus, MfaProvider mfaProviders)
            : base(userId, authenticationStatus)
        {
            this.SetupMfaProviders = mfaProviders;
        }

        public AuthenticateUserCommandResult(Guid userId, MfaProvider mfaProviders, AssertionOptions assertionOptions)
            : base(userId, AuthenticationState.AwaitingMfaDeviceCode)
        {
            this.SetupMfaProviders = mfaProviders;
            this._assertionOptions = assertionOptions;
        }

        public MfaProvider SetupMfaProviders { get; }

        public AssertionOptions AssertionOptions
        {
            get
            {
                if (this.AuthenticationStatus != AuthenticationState.AwaitingMfaDeviceCode)
                {
                    throw new InvalidOperationException();
                }

                return this._assertionOptions;
            }
        }
    }
}