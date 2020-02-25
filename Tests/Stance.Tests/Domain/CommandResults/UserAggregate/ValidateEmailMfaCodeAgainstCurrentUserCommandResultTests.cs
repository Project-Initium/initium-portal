// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Domain.CommandResults.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandResults.UserAggregate
{
    public class ValidateEmailMfaCodeAgainstCurrentUserCommandResultTests
    {
        [Fact]
        public void AuthenticatedConstructor_GiveValidArguments_PropertiesAreSet()
        {
            var userId = Guid.NewGuid();
            var commandResult = new ValidateEmailMfaCodeAgainstCurrentUserCommandResult(userId, new string('*', 6), new string('*', 7), new string('*', 8));
            Assert.Equal(new string('*', 6), commandResult.EmailAddress);
            Assert.Equal(new string('*', 7), commandResult.FirstName);
            Assert.Equal(new string('*', 8), commandResult.LastName);
            Assert.Equal(userId, commandResult.UserId);
            Assert.Equal(BaseAuthenticationProcessCommandResult.AuthenticationState.Completed, commandResult.AuthenticationStatus);
        }

        [Fact]
        public void PartiallyAuthenticatedConstructor_GiveValidArguments_PropertiesAreSet()
        {
            var userId = Guid.NewGuid();
            var commandResult = new ValidateEmailMfaCodeAgainstCurrentUserCommandResult(userId, BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaEmailCode);
            Assert.Equal(userId, commandResult.UserId);
            Assert.Equal(BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaEmailCode, commandResult.AuthenticationStatus);
            Assert.Throws<InvalidOperationException>(() => commandResult.EmailAddress);
            Assert.Throws<InvalidOperationException>(() => commandResult.FirstName);
            Assert.Throws<InvalidOperationException>(() => commandResult.LastName);
        }
    }
}