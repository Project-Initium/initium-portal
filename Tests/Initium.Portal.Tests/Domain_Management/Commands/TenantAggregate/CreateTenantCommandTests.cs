// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Xunit;

namespace Initium.Portal.Tests.DomainManagement.Commands.TenantAggregate
{
    public class CreateTenantCommandTests
    {
        [Fact]
        public void Constructor_WhenValid_ExpectPropertiesAreSet()
        {
            var cmd = new CreateTenantCommand(TestVariables.TenantId, "identifier", "name", "connection-string");

            Assert.Equal(TestVariables.TenantId, cmd.TenantId);
            Assert.Equal("identifier", cmd.Identifier);
            Assert.Equal("name", cmd.Name);
            Assert.Equal("connection-string", cmd.ConnectionString);
        }
    }
}