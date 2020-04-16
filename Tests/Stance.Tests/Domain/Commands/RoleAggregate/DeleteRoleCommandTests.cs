// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Domain.Commands.RoleAggregate;
using Xunit;

namespace Stance.Tests.Domain.Commands.RoleAggregate
{
    public class DeleteRoleCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var command = new DeleteRoleCommand(TestVariables.RoleId);
            Assert.Equal(TestVariables.RoleId, command.RoleId);
        }
    }
}