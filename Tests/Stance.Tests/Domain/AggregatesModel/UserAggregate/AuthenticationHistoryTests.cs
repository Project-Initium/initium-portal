// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Core.Constants;
using Stance.Domain.AggregatesModel.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.AggregatesModel.UserAggregate
{
    public class AuthenticationHistoryTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var id = Guid.NewGuid();
            var whenHappened = DateTime.UtcNow;
            var authenticationHistory = new AuthenticationHistory(
                id,
                whenHappened,
                AuthenticationHistoryType.Success);

            Assert.Equal(id, authenticationHistory.Id);
            Assert.Equal(whenHappened, authenticationHistory.WhenHappened);
            Assert.Equal(AuthenticationHistoryType.Success, authenticationHistory.AuthenticationHistoryType);
        }
    }
}