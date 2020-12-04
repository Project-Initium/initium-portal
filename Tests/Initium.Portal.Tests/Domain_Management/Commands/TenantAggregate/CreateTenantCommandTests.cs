// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Initium.Portal.Core.Constants;
using Xunit;

namespace Initium.Portal.Tests.Domain_Management.Commands.TenantAggregate
{
    public class CreateTenantCommandTests
    {
        [Fact]
        public void Constructor_WhenValid_ExpectPropertiesAreSet()
        {
            var cmd = new CreateTenantCommand(
                TestVariables.TenantId,
                "identifier",
                "name",
                "connection-string",
                new List<SystemFeatures>
                {
                    SystemFeatures.MfaApp,
                });

            Assert.Equal(TestVariables.TenantId, cmd.TenantId);
            Assert.Equal("identifier", cmd.Identifier);
            Assert.Equal("name", cmd.Name);
            Assert.Equal("connection-string", cmd.ConnectionString);
            Assert.Single(cmd.SystemFeatures);
        }
    }
}