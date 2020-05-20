// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Constants;
using Initium.Portal.Domain.Commands.NotificationAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.Commands.NotificationAggregate
{
    public class CreateNotificationCommandTests
    {
        [Fact]
        public void Constructor_WhenValid_ExpectPropertiesAreSet()
        {
            var command = new CreateNotificationCommand(
                "subject",
                "message",
                NotificationType.AlphaNotification,
                "serialized-event-data",
                TestVariables.Now,
                new List<Guid>
                {
                    TestVariables.UserId,
                });

            Assert.Equal("subject", command.Subject);
            Assert.Equal("message", command.Message);
            Assert.Equal(NotificationType.AlphaNotification, command.Type);
            Assert.Equal("serialized-event-data", command.SerializedEventData);
            Assert.Equal(TestVariables.Now, command.WhenNotified);
            Assert.NotNull(command.UserIds);
            Assert.Contains(command.UserIds, x => x == TestVariables.UserId);
        }
    }
}