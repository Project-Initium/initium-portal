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
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var userId = Guid.NewGuid();
            var commandResult = new AuthenticateUserCommandResult(userId, new string('*', 6));
            Assert.Equal(new string('*', 6), commandResult.EmailAddress);
            Assert.Equal(userId, commandResult.UserId);
        }
    }
}