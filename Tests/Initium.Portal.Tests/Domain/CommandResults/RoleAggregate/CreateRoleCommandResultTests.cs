// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Domain.CommandResults.RoleAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandResults.RoleAggregate
{
    public class CreateRoleCommandResultTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var commandResult = new CreateRoleCommandResult(TestVariables.RoleId);
            Assert.Equal(TestVariables.RoleId, commandResult.RoleId);
        }
    }
}