// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Initium.Portal.Core.Constants;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.AggregatesModel.UserAggregate
{
    public class AuthenticationHistoryTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var authenticationHistory = new AuthenticationHistory(
                TestVariables.AuthenticationHistoryId,
                TestVariables.Now,
                AuthenticationHistoryType.Success);

            Assert.Equal(TestVariables.AuthenticationHistoryId, authenticationHistory.Id);
            Assert.Equal(TestVariables.Now, authenticationHistory.WhenHappened);
            Assert.Equal(AuthenticationHistoryType.Success, authenticationHistory.AuthenticationHistoryType);

            foreach (var prop in authenticationHistory.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(authenticationHistory, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void Constructor_GivenPrivateIsCalled_ExpectObjectCreated()
        {
            var authenticationHistory = (AuthenticationHistory)Activator.CreateInstance(typeof(AuthenticationHistory), true);
            Assert.NotNull(authenticationHistory);

            foreach (var prop in authenticationHistory.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(authenticationHistory, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }
    }
}