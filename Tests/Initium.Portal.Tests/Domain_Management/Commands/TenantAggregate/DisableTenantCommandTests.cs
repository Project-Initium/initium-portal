// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Xunit;

namespace Initium.Portal.Tests.DomainManagement.Commands.TenantAggregate
{
    public class DisableTenantCommandTests
    {
        [Fact]
        public void Constructor_WhenValid_ExpectPropertiesAreSet()
        {
            var cmd = new DisableTenantCommand(TestVariables.TenantId);

            Assert.Equal(TestVariables.TenantId, cmd.TenantId);
        }
    }
}