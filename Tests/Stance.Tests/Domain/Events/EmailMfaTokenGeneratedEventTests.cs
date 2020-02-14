// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Domain.Events;
using Xunit;

namespace Stance.Tests.Domain.Events
{
    public class EmailMfaTokenGeneratedEventTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var id = Guid.NewGuid();
            var email = new string('*', 5);
            var generated = new string('*', 6);
            var @event = new EmailMfaTokenGeneratedEvent(id, email, generated);

            Assert.Equal(id, @event.UserId);
            Assert.Equal(email, @event.UserEmailAddress);
            Assert.Equal(generated, @event.Generated);
        }
    }
}