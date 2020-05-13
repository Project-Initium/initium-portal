// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Stance.Domain.Events;
using Xunit;

namespace Stance.Tests.Domain.Events
{
    public class UserEnabledEventTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var @event = new UserEnabledEvent("email-address", "first-name", "last-name");

            Assert.Equal("email-address", @event.EmailAddress);
            Assert.Equal("first-name", @event.FirstName);
            Assert.Equal("last-name", @event.LastName);
        }
    }
}