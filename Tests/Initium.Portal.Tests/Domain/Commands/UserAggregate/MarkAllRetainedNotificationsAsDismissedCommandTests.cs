// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Domain.Commands.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.Commands.UserAggregate
{
    public class MarkAllRetainedNotificationsAsDismissedCommandTests
    {
        [Fact]
        public void Constructor_WhenValid_ExpectPropertiesAreSet()
        {
            var command = new MarkAllRetainedNotificationsAsDismissedCommand(
                TestVariables.UserId);

            Assert.Equal(TestVariables.UserId, command.UserId);
        }
    }
}