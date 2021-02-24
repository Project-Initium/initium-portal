// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Domain.Events.IntegrationEvents;
using Xunit;

namespace Initium.Portal.Tests.Domain.Events.IntegrationEvents
{
    public class GenerateAccountConfirmationTokenGeneratedIntegrationEventTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var @event =
                new AccountConfirmationTokenGeneratedIntegrationEvent(
                    "email-address",
                    "first-name",
                    "last-name",
                    TestVariables.SecurityTokenMappingId,
                    TestVariables.Now);

            Assert.Equal("email-address", @event.EmailAddress);
            Assert.Equal("first-name", @event.FirstName);
            Assert.Equal("last-name", @event.LastName);
            Assert.Equal(TestVariables.SecurityTokenMappingId, @event.Token);
            Assert.Equal(TestVariables.Now, @event.WhenExpires);
        }
    }
}