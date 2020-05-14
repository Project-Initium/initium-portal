// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.AggregatesModel.UserAggregate
{
    public class UserRoleTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var userRole = new UserRole(TestVariables.RoleId);
            Assert.Equal(TestVariables.RoleId, userRole.Id);

            foreach (var prop in userRole.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(userRole, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void Constructor_GivenPrivateIsCalled_ExpectObjectCreated()
        {
            var userRole = (UserRole)Activator.CreateInstance(typeof(UserRole), true);
            Assert.NotNull(userRole);

            foreach (var prop in userRole.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(userRole, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }
    }
}