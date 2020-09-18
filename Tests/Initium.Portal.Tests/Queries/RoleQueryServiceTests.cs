// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Initium.Portal.Queries;
using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Queries
{
    public class RoleQueryServiceTests
    {
        [Fact]
        public async Task CheckForPresenceOfRoleByName_GivenRoleDoesNotExist_ExpectNotPresentStatus()
        {
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new Mock<ITenantInfo>();
            tenantInfo.Setup(x => x.Id).Returns(TestVariables.TenantId.ToString);

            await using var context = new QueryContext(options, tenantInfo.Object);
            var roleQueries = new RoleQueryService(context);
            var result = await roleQueries.CheckForPresenceOfRoleByName("name");
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfRoleByName_GivenRoleDoesExist_ExpectPresentStatus()
        {
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new Mock<ITenantInfo>();
            tenantInfo.Setup(x => x.Id).Returns(TestVariables.TenantId.ToString);

            await using var context = new QueryContext(options, tenantInfo.Object);
            var role = context.Add(new Role
            {
                Id = TestVariables.RoleId,
                Name = "name",
            });
            role.Property("TenantId").CurrentValue = TestVariables.TenantId;
            await context.SaveChangesAsync();

            var roleQueries = new RoleQueryService(context);
            var result = await roleQueries.CheckForPresenceOfRoleByName("name");
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task CheckForRoleUsageById_GivenRoleIsInUse_ExpectPresentStatus()
        {
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new Mock<ITenantInfo>();
            tenantInfo.Setup(x => x.Id).Returns(TestVariables.TenantId.ToString);

            await using var context = new QueryContext(options, tenantInfo.Object);
            var role = context.Add(new Role
            {
                Id = TestVariables.RoleId,
                ResourceCount = 1,
            });
            role.Property("TenantId").CurrentValue = TestVariables.TenantId;
            await context.SaveChangesAsync();

            var roleQueries = new RoleQueryService(context);
            var result = await roleQueries.CheckForRoleUsageById(TestVariables.RoleId);
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task CheckForRoleUsageById_GivenRoleIsNotInUse_ExpectNotPresentStatus()
        {
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new Mock<ITenantInfo>();
            tenantInfo.Setup(x => x.Id).Returns(TestVariables.TenantId.ToString);

            await using var context = new QueryContext(options, tenantInfo.Object);
            var role = context.Add(new Role
            {
                Id = TestVariables.RoleId,
            });
            role.Property("TenantId").CurrentValue = TestVariables.TenantId;
            var roleQueries = new RoleQueryService(context);
            var result = await roleQueries.CheckForRoleUsageById(TestVariables.RoleId);
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task GetDetailsOfRoleById_GivenDataIsFound_ExpectMaybeWithMappedData()
        {
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new Mock<ITenantInfo>();
            tenantInfo.Setup(x => x.Id).Returns(TestVariables.TenantId.ToString);

            await using var context = new QueryContext(options, tenantInfo.Object);

            var roleQueries = new RoleQueryService(context);
            var result = await roleQueries.GetDetailsOfRoleById(TestVariables.RoleId);
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetDetailsOfRoleById_GivenNoDataIsFound_ExpectMaybeWithNothing()
        {
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new Mock<ITenantInfo>();
            tenantInfo.Setup(x => x.Id).Returns(TestVariables.TenantId.ToString);

            await using var context = new QueryContext(options, tenantInfo.Object);
            var roleResource = await context.RoleResources.AddAsync(new RoleResource
            {
                Resource = new Resource
                {
                    Id = TestVariables.ResourceId,
                },
            });
            roleResource.Property("TenantId").CurrentValue = TestVariables.TenantId;

            var role = await context.Roles.AddAsync(new Role
            {
                Id = TestVariables.RoleId,
                Name = "name",
                RoleResources = new List<RoleResource>
                {
                    roleResource.Entity,
                },
            });
            role.Property("TenantId").CurrentValue = TestVariables.TenantId;

            await context.SaveChangesAsync();

            var roleQueries = new RoleQueryService(context);
            var result = await roleQueries.GetDetailsOfRoleById(TestVariables.RoleId);
            Assert.True(result.HasValue);
            Assert.Equal(TestVariables.RoleId, result.Value.Id);
            Assert.Equal("name", result.Value.Name);
            var res = Assert.Single(result.Value.Resources);
            Assert.Equal(TestVariables.ResourceId, res);
        }

        [Fact]
        public async Task GetSimpleRoles_GivenNoDataIsFound_ExpectMaybeWithNothing()
        {
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new Mock<ITenantInfo>();
            tenantInfo.Setup(x => x.Id).Returns(TestVariables.TenantId.ToString);

            await using var context = new QueryContext(options, tenantInfo.Object);

            var roleQueries = new RoleQueryService(context);
            var result = await roleQueries.GetSimpleRoles();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetSimpleRoles_GivenDataIsFound_ExpectMaybeWithMappedData()
        {
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new Mock<ITenantInfo>();
            tenantInfo.Setup(x => x.Id).Returns(TestVariables.TenantId.ToString);

            await using var context = new QueryContext(options, tenantInfo.Object);
            var role = context.Add(new Role
            {
                Id = TestVariables.RoleId,
                Name = "name",
            });
            role.Property("TenantId").CurrentValue = TestVariables.TenantId;

            await context.SaveChangesAsync();

            var roleQueries = new RoleQueryService(context);
            var result = await roleQueries.GetSimpleRoles();
            Assert.True(result.HasValue);
            Assert.Single(result.Value);
            Assert.Equal("name", result.Value.First().Name);
            Assert.Equal(TestVariables.RoleId, result.Value.First().Id);
        }
    }
}