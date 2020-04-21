// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Stance.Domain.Events;
using Xunit;

namespace Stance.Tests.Domain.Events
{
    public class PasswordResetTokenGeneratedEventTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var @event = new PasswordResetTokenGeneratedEvent("email-address", "first-name", "last-name", "token");

            Assert.Equal("email-address", @event.EmailAddress);
            Assert.Equal("first-name", @event.FirstName);
            Assert.Equal("last-name", @event.LastName);
            Assert.Equal("token", @event.Token);
        }
    }
}