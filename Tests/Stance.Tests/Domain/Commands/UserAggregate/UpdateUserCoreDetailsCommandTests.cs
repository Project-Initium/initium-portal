// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Stance.Domain.Commands.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.Commands.UserAggregate
{
    public class UpdateUserCoreDetailsCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var command = new UpdateUserCoreDetailsCommand(TestVariables.UserId, "email-address", "first-name",
                "last-name", false, true, new List<Guid> { TestVariables.RoleId });
            Assert.Equal(TestVariables.UserId, command.UserId);
            Assert.Equal("email-address", command.EmailAddress);
            Assert.Equal("first-name", command.FirstName);
            Assert.Equal("last-name", command.LastName);
            Assert.False(command.IsLockable);
            Assert.Single(command.Roles);
        }
    }
}