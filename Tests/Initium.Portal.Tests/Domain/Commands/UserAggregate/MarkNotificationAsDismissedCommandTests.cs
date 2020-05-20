// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Domain.Commands.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.Commands.UserAggregate
{
    public class MarkNotificationAsDismissedCommandTests
    {
        [Fact]
        public void Constructor_WhenValid_ExpectPropertiesAreSet()
        {
            var command = new MarkNotificationAsDismissedCommand(
                TestVariables.UserId,
                TestVariables.NotificationId);

            Assert.Equal(TestVariables.UserId, command.UserId);
            Assert.Equal(TestVariables.NotificationId, command.NotificationId);
        }
    }
}