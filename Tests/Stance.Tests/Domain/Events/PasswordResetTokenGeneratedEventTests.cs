// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Domain.Events;
using Xunit;

namespace Stance.Tests.Domain.Events
{
    public class PasswordResetTokenGeneratedEventTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var id = Guid.NewGuid();
            var email = new string('*', 5);
            var token = new string('*', 6);
            var @event = new PasswordResetTokenGeneratedEvent(id, email, token);

            Assert.Equal(id, @event.UserId);
            Assert.Equal(email, @event.EmailAddress);
            Assert.Equal(token, @event.Token);
        }
    }
}