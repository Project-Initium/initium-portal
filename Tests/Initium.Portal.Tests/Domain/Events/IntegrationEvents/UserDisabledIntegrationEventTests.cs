// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Domain.Events.IntegrationEvents;
using Xunit;

namespace Initium.Portal.Tests.Domain.Events.IntegrationEvents
{
    public class UserDisabledIntegrationEventTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var @event = new UserDisabledIntegrationEvent("email-address", "first-name", "last-name");

            Assert.Equal("email-address", @event.EmailAddress);
            Assert.Equal("first-name", @event.FirstName);
            Assert.Equal("last-name", @event.LastName);
        }
    }
}