// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Stance.Domain.Commands.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.Commands.UserAggregate
{
    public class CreateUserCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var command = new CreateUserCommand(new string('*', 5), new string('*', 6), new string('*', 7), false, true, new List<Guid>());
            Assert.Equal(new string('*', 5), command.EmailAddress);
            Assert.Equal(new string('*', 6), command.FirstName);
            Assert.Equal(new string('*', 7), command.LastName);
            Assert.False(command.IsLockable);
        }
    }
}