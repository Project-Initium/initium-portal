﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Core.Settings;
using Initium.Portal.Queries;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Management;
using Initium.Portal.Web.ODataEndpoints.Role;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Web.ODataEndpoints.Role
{
    public class RoleODataControllerTests : BaseODataControllerTest<Portal.Queries.Entities.RoleReadEntity>
    {
        public static IEnumerable<object[]> FilterData
        {
            get
            {
                return new[]
                {
                    new object[] { new RoleFilter { HasResources = true }, 2 },
                    new object[] { new RoleFilter { HasResources = true, HasNoResources = true }, 4 },
                    new object[] { new RoleFilter { HasNoResources = true }, 2 },
                    new object[] { new RoleFilter { HasUsers = true }, 1 },
                    new object[] { new RoleFilter { HasUsers = true, HasNoUsers = true }, 4 },
                    new object[] { new RoleFilter { HasNoUsers = true }, 3 },
                };
            }
        }

        protected override IEdmModel EdmModel
        {
            get
            {
                var modelBuilder = new ODataConventionModelBuilder(this.Provider);
                var entitySet = modelBuilder.EntitySet<Portal.Queries.Entities.RoleReadEntity>("Role");
                entitySet.EntityType.HasKey(entity => entity.Id);
                return modelBuilder.GetEdmModel();
            }
        }

        [Theory]
        [MemberData(nameof(FilterData))]
        public void Filtered_GivenFilterIsNotNull_ExpectFilteredData(RoleFilter filter, int count)
        {
            var options = new DbContextOptionsBuilder<CoreQueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new FeatureBasedTenantInfo
            {
                Id = TestVariables.TenantId.ToString(),
            };

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            using var context = new ManagementQueryContext(options, tenantInfo, multiTenantSettings.Object);
            context.Add(new Portal.Queries.Entities.RoleReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "Role-1",
                ResourceCount = 0,
                UserCount = 3,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.RoleReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "Role-2",
                ResourceCount = 2,
                UserCount = 0,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.RoleReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "Role-3",
                ResourceCount = 4,
                UserCount = 0,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.RoleReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "Role-4",
                ResourceCount = 0,
                UserCount = 0,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.SaveChanges();
            var roleQueryService = new Mock<IRoleQueryService>();
            roleQueryService.Setup(x => x.QueryableEntity).Returns(context.Roles);

            var roleController = new RoleODataController(roleQueryService.Object);
            var result = Assert.IsType<OkObjectResult>(roleController.Filtered(this.GenerateEmptyQueryOptions(), filter));
            var data = Assert.IsType<EntityQueryable<Portal.Queries.Entities.RoleReadEntity>>(result.Value);
            Assert.Equal(count, data.Count());
        }

        [Fact]
        public void Filtered_GivenFilterIsNull_ExpectUnfiltered()
        {
            var options = new DbContextOptionsBuilder<CoreQueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new FeatureBasedTenantInfo
            {
                Id = TestVariables.TenantId.ToString(),
            };

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            using var context = new ManagementQueryContext(options, tenantInfo, multiTenantSettings.Object);
            context.Add(new Portal.Queries.Entities.RoleReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "Role-1",
                ResourceCount = 0,
                UserCount = 3,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.RoleReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "Role-2",
                ResourceCount = 2,
                UserCount = 0,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.RoleReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "Role-3",
                ResourceCount = 4,
                UserCount = 4,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.RoleReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "Role-4",
                ResourceCount = 0,
                UserCount = 0,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.SaveChanges();

            var roleQueryService = new Mock<IRoleQueryService>();
            roleQueryService.Setup(x => x.QueryableEntity).Returns(context.Roles);
            var roleController = new RoleODataController(roleQueryService.Object);
            var result = Assert.IsType<OkObjectResult>(roleController.Filtered(this.GenerateEmptyQueryOptions(), null));
            var data = Assert.IsType<InternalDbSet<Portal.Queries.Entities.RoleReadEntity>>(result.Value);
            Assert.Equal(4, data.AsQueryable().Count());
        }
    }
}