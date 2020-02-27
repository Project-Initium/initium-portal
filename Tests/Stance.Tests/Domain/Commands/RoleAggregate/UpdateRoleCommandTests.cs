// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Stance.Domain.Commands.RoleAggregate;
using Xunit;

namespace Stance.Tests.Domain.Commands.RoleAggregate
{
    public class UpdateRoleCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var id = Guid.NewGuid();
            var command = new UpdateRoleCommand(id, new string('*', 5), new List<Guid> { Guid.NewGuid() });
            Assert.Equal(id, command.RoleId);
            Assert.Equal(new string('*', 5), command.Name);
            Assert.Single(command.Resources);
        }
    }
}