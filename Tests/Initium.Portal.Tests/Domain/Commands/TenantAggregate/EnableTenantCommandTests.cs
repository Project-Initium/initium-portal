// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Domain.Commands.TenantAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.Commands.TenantAggregate
{
    public class EnableTenantCommandTests
    {
        [Fact]
        public void Constructor_WhenValid_ExpectPropertiesAreSet()
        {
            var cmd = new EnableTenantCommand(TestVariables.TenantId);

            Assert.Equal(TestVariables.TenantId, cmd.TenantId);
        }
    }
}