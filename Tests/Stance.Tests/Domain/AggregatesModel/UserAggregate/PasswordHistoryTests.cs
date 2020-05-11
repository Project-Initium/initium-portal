// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Stance.Domain.AggregatesModel.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.AggregatesModel.UserAggregate
{
    public class PasswordHistoryTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var passwordHistory = new PasswordHistory(TestVariables.PasswordHistoryId, "hash", TestVariables.Now);

            Assert.Equal(TestVariables.PasswordHistoryId, passwordHistory.Id);
            Assert.Equal("hash", passwordHistory.Hash);
            Assert.Equal(TestVariables.Now, passwordHistory.WhenUsed);

            foreach (var prop in passwordHistory.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(passwordHistory, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void Constructor_GivenPrivateIsCalled_ExpectObjectCreated()
        {
            var passwordHistory = (PasswordHistory)Activator.CreateInstance(typeof(PasswordHistory), true);
            Assert.NotNull(passwordHistory);

            foreach (var prop in passwordHistory.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(passwordHistory, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }
    }
}