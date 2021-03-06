﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Domain.Commands.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.Commands.UserAggregate
{
    public class CreateUserCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var command = new CreateUserCommand("email-address", "first-name", "last-name", false, true,
                new List<Guid> { TestVariables.RoleId });
            Assert.Equal("email-address", command.EmailAddress);
            Assert.Equal("first-name", command.FirstName);
            Assert.Equal("last-name", command.LastName);
            Assert.Single(command.Roles);
            Assert.False(command.IsLockable);
        }
    }
}