// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Domain.CommandResults.RoleAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandResults.RoleAggregate
{
    public class CreateRoleCommandResultTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var id = Guid.NewGuid();
            var commandResult = new CreateRoleCommandResult(id);
            Assert.Equal(id, commandResult.RoleId);
        }
    }
}