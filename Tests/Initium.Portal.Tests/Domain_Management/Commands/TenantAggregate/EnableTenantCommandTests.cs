// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain_Management.Commands.TenantAggregate
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