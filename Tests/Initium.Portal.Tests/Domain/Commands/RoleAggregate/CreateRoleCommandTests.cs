// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Domain.Commands.RoleAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.Commands.RoleAggregate
{
    public class CreateRoleCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var command = new CreateRoleCommand("name", new List<Guid> { TestVariables.ResourceId });
            Assert.Equal("name", command.Name);
            Assert.Single(command.Resources);
        }
    }
}