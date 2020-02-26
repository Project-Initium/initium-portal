// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Domain.Commands.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.Commands.UserAggregate
{
    public class UpdateUserCoreDetailsCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var id = Guid.NewGuid();
            var command = new UpdateUserCoreDetailsCommand(id, new string('*', 5), new string('*', 6),
                new string('*', 7), false);
            Assert.Equal(id, command.UserId);
            Assert.Equal(new string('*', 5), command.EmailAddress);
            Assert.Equal(new string('*', 6), command.FirstName);
            Assert.Equal(new string('*', 7), command.LastName);
            Assert.False(command.IsLockable);
        }
    }
}