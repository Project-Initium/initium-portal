// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.AggregatesModel.UserAggregate
{
    public class UserNotificationTests
    {
        [Fact]
        public void MarkAsViewed_GivenValidArguments_ExpectWhenViewedToBeSet()
        {
            var userNotification = (UserNotification)Activator.CreateInstance(typeof(UserNotification), true);
            Assert.NotNull(userNotification);
            userNotification.MarkAsViewed(TestVariables.Now);
            Assert.Equal(TestVariables.Now, userNotification.WhenViewed);
        }

        [Fact]
        public void MarkAsDismissed_WGivenValidArguments_ExpectWhenViewedToBeSet()
        {
            var userNotification = (UserNotification)Activator.CreateInstance(typeof(UserNotification), true);
            Assert.NotNull(userNotification);
            userNotification.MarkAsDismissed(TestVariables.Now);
            Assert.Equal(TestVariables.Now, userNotification.WhenDismissed);
        }

        [Fact]
        public void Constructor_GivenValidArguments_ExpectNavigationalPropertiesCreated()
        {
            var userNotification = (UserNotification)Activator.CreateInstance(typeof(UserNotification), true);
            Assert.NotNull(userNotification);

            foreach (var prop in userNotification.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(userNotification, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }
    }
}