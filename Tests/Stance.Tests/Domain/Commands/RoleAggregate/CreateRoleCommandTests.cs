// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Stance.Domain.Commands.RoleAggregate;
using Xunit;

namespace Stance.Tests.Domain.Commands.RoleAggregate
{
    public class CreateRoleCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var command = new CreateRoleCommand(new string('*', 5), new List<Guid> { Guid.NewGuid() });
            Assert.Equal(new string('*', 5), command.Name);
            Assert.Single(command.Resources);
        }
    }
}