// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Domain.CommandResults.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandResults.UserAggregate
{
    public class AuthenticateUserCommandResultTests
    {
        [Fact]
        public void AuthenticatedConstructor_GiveValidArguments_PropertiesAreSet()
        {
            var userId = Guid.NewGuid();
            var commandResult = new AuthenticateUserCommandResult(userId);
            Assert.Equal(userId, commandResult.UserId);
            Assert.Equal(BaseAuthenticationProcessCommandResult.AuthenticationState.Completed, commandResult.AuthenticationStatus);
        }

        [Fact]
        public void PartiallyAuthenticatedConstructor_GiveValidArguments_PropertiesAreSet()
        {
            var userId = Guid.NewGuid();
            var commandResult = new AuthenticateUserCommandResult(userId, BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaEmailCode);
            Assert.Equal(userId, commandResult.UserId);
            Assert.Equal(BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaEmailCode, commandResult.AuthenticationStatus);
        }
    }
}