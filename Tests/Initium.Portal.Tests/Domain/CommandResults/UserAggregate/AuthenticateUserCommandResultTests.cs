// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Fido2NetLib;
using Initium.Portal.Core.Constants;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandResults.UserAggregate
{
    public class AuthenticateUserCommandResultTests
    {
        [Fact]
        public void PartiallyAuthenticatedDeviceConstructor_GiveValidArguments_PropertiesAreSet()
        {
            var assertionOptions = new AssertionOptions();
            var commandResult = new AuthenticateUserCommandResult(TestVariables.UserId, MfaProvider.Device, assertionOptions);
            Assert.Equal(TestVariables.UserId, commandResult.UserId);
            Assert.Equal(BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaDeviceCode, commandResult.AuthenticationStatus);
            Assert.Equal(MfaProvider.Device, commandResult.SetupMfaProviders);
            Assert.Equal(assertionOptions, commandResult.AssertionOptions);
        }

        [Fact]
        public void PartiallyAuthenticatedConstructor_GiveValidArguments_PropertiesAreSet()
        {
            var commandResult = new AuthenticateUserCommandResult(TestVariables.UserId, BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaEmailCode, MfaProvider.App);
            Assert.Equal(TestVariables.UserId, commandResult.UserId);
            Assert.Equal(BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaEmailCode, commandResult.AuthenticationStatus);
            Assert.True(commandResult.SetupMfaProviders.HasFlag(MfaProvider.App));
            Assert.Throws<InvalidOperationException>(() => commandResult.AssertionOptions);
        }
    }
}