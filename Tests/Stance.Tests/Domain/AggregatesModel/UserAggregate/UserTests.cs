// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Domain.AggregatesModel.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.AggregatesModel.UserAggregate
{
    public sealed class UserTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var id = Guid.NewGuid();
            var whenCreated = DateTime.UtcNow;
            var user = new User(
                id,
                new string('*', 5),
                new string('*', 6),
                whenCreated);

            Assert.Equal(id, user.Id);
            Assert.Equal(new string('*', 5), user.EmailAddress);
            Assert.Equal(new string('*', 6), user.PasswordHash);
            Assert.Equal(whenCreated, user.WhenCreated);
        }
    }
}