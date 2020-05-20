// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Domain.CommandResults.NotificationAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandResults.NotificationAggregate
{
    public class CreateNotificationCommandResultTests
    {
        [Fact]
        public void Constructor_WhenValid_ExpectPropertiesAreSet()
        {
            var commandResult = new CreateNotificationCommandResult(TestVariables.NotificationId);

            Assert.Equal(TestVariables.NotificationId, commandResult.NotificationId);
        }
    }
}