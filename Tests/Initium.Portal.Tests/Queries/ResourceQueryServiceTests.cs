// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Initium.Portal.Core.Settings;
using Initium.Portal.Queries;
using Initium.Portal.Queries.Entities;
using Initium.Portal.Queries.Management;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Queries
{
    public class ResourceQueryServiceTests
    {
        [Fact]
        public async Task GetNestedSimpleResources_GivenDataIsFound_ExpectMaybeWithMappedData()
        {
            var options = new DbContextOptionsBuilder<CoreQueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new Mock<ITenantInfo>();

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            await using var context = new ManagementQueryContext(options, tenantInfo.Object, multiTenantSettings.Object);
            context.Add(new ResourceReadEntity
            {
                Id = TestVariables.ResourceId,
                Name = "parent-1",
            });
            context.Add(new ResourceReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "parent-2",
            });
            var childId = Guid.NewGuid();
            context.Add(new ResourceReadEntity
            {
                Id = childId,
                Name = "child-1",
                ParentResourceId = TestVariables.ResourceId,
            });
            context.SaveChanges();

            var roleQueries = new ResourceQueryService(context);
            var result = await roleQueries.GetNestedSimpleResources();
            Assert.True(result.HasValue);
            Assert.Equal(2, result.Value.Count);
            var parent1 = result.Value.FirstOrDefault(x => x.Name == "parent-1");
            Assert.NotNull(parent1);
            Assert.Equal("parent-1", parent1.Name);
            Assert.Equal(TestVariables.ResourceId, parent1.Id);
            Assert.Single(parent1.SimpleResources);
            Assert.Equal("child-1", parent1.SimpleResources.First().Name);
            Assert.Equal(childId, parent1.SimpleResources.First().Id);
        }

        [Fact]
        public async Task GetNestedSimpleResources_GivenNoDataIsFound_ExpectMaybeWithNothing()
        {
            var options = new DbContextOptionsBuilder<CoreQueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new Mock<ITenantInfo>();

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            await using var context = new ManagementQueryContext(options, tenantInfo.Object, multiTenantSettings.Object);

            var roleQueries = new ResourceQueryService(context);
            var result = await roleQueries.GetNestedSimpleResources();
            Assert.True(result.HasNoValue);
        }
    }
}