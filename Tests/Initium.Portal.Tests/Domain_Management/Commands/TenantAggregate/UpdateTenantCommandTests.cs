// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Initium.Portal.Core.Constants;
using Xunit;

namespace Initium.Portal.Tests.Domain_Management.Commands.TenantAggregate
{
    public class UpdateTenantCommandTests
    {
        [Fact]
        public void Constructor_WhenValid_ExpectPropertiesAreSet()
        {
            var cmd = new UpdateTenantCommand(
                TestVariables.TenantId,
                "identifier",
                "name",
                new List<SystemFeatures>
                {
                    SystemFeatures.MfaApp,
                });

            Assert.Equal(TestVariables.TenantId, cmd.TenantId);
            Assert.Equal("identifier", cmd.Identifier);
            Assert.Equal("name", cmd.Name);
            Assert.Single(cmd.SystemFeatures);
        }
    }
}