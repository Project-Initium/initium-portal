// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Initium.Portal.Core.Constants;
using Initium.Portal.Domain.AggregatesModel.NotificationAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.AggregatesModel.NotificationAggregate
{
    public class NotificationAggregate
    {
        [Fact]
        public void Constructor_WhenEmpty_ExpectNavigationalPropertiesCreated()
        {
            var notification = (Notification)Activator.CreateInstance(typeof(Notification), true);

            foreach (var prop in notification.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(notification, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void Constructor_WhenValid_ExpectPropertiesAreSet()
        {
            var notification = new Notification(
                TestVariables.NotificationId,
                "subject",
                "message",
                NotificationType.Test,
                "serialized-event-data",
                TestVariables.Now);

            Assert.Equal(TestVariables.NotificationId, notification.Id);
            Assert.Equal("subject", notification.Subject);
            Assert.Equal("message", notification.Message);
            Assert.Equal(NotificationType.Test, notification.Type);
            Assert.Equal("serialized-event-data", notification.SerializedEventData);
            Assert.Equal(TestVariables.Now, notification.WhenNotified);
            Assert.NotNull(notification.UserNotifications);

            foreach (var prop in notification.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(notification, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void SendToUser_WhenValid_ExpectNewUserNotificationInList()
        {
            var notification = new Notification(
                TestVariables.NotificationId,
                "subject",
                "message",
                NotificationType.Test,
                "serialized-event-data",
                TestVariables.Now);

            var userId = Guid.NewGuid();
            notification.SendToUser(userId);

            Assert.NotNull(notification.UserNotifications);
            Assert.Contains(notification.UserNotifications, x => x.Id == userId);
        }
    }
}