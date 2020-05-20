// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Initium.Portal.Domain.AggregatesModel.NotificationAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.AggregatesModel.NotificationAggregate
{
    public class UserNotificationTests
    {
        [Fact]
        public void Constructor_WhenValid_ExpectPropertiesAreSet()
        {
            var userNotification = new UserNotification(TestVariables.UserId);
            Assert.Equal(TestVariables.UserId, userNotification.Id);

            foreach (var prop in userNotification.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(userNotification, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void Constructor_WhenEmpty_ExpectNavigationalPropertiesCreated()
        {
            var userNotification = (UserNotification)Activator.CreateInstance(typeof(UserNotification), true);

            foreach (var prop in userNotification.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(userNotification, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }
    }
}