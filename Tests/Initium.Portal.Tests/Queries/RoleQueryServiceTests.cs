// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Core.Settings;
using Initium.Portal.Queries;
using Initium.Portal.Queries.Entities;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace Initium.Portal.Tests.Queries
{
    public class RoleQueryServiceTests
    {
        [Fact]
        public async Task CheckForPresenceOfRoleByName_GivenRoleDoesNotExist_ExpectNotPresentStatus()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<RoleReadEntity>())
                .ReturnsDbSet(new List<RoleReadEntity>());
            var roleQueries = new RoleQueryService(context.Object);
            var result = await roleQueries.CheckForPresenceOfRoleByName("name");
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfRoleByName_GivenRoleDoesExist_ExpectPresentStatus()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var entity = Helpers.CreateEntity<RoleReadEntity>(new
            {
                Id = TestVariables.RoleId,
                Name = "name",
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<RoleReadEntity>())
                .ReturnsDbSet(new List<RoleReadEntity>
                {
                    entity,
                });

            var roleQueries = new RoleQueryService(context.Object);
            var result = await roleQueries.CheckForPresenceOfRoleByName("name");
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task CheckForRoleUsageById_GivenRoleIsInUse_ExpectPresentStatus()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<RoleReadEntity>())
                .ReturnsDbSet(new List<RoleReadEntity>
                {
                    Helpers.CreateEntity<RoleReadEntity>(new
                    {
                        Id = TestVariables.RoleId,
                        ResourceCount = 1,
                    }),
                });

            var roleQueries = new RoleQueryService(context.Object);
            var result = await roleQueries.CheckForRoleUsageById(TestVariables.RoleId);
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task CheckForRoleUsageById_GivenRoleIsNotInUse_ExpectNotPresentStatus()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<RoleReadEntity>())
                .ReturnsDbSet(new List<RoleReadEntity>
                {
                    Helpers.CreateEntity<RoleReadEntity>(new
                    {
                        Id = TestVariables.RoleId,
                    }),
                });

            var roleQueries = new RoleQueryService(context.Object);
            var result = await roleQueries.CheckForRoleUsageById(TestVariables.RoleId);
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task GetDetailsOfRoleById_GivenDataIsFound_ExpectMaybeWithMappedData()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<RoleReadEntity>())
                .ReturnsDbSet(new List<RoleReadEntity>());

            var roleQueries = new RoleQueryService(context.Object);
            var result = await roleQueries.GetDetailsOfRoleById(TestVariables.RoleId);
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetDetailsOfRoleById_GivenNoDataIsFound_ExpectMaybeWithNothing()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<RoleReadEntity>())
                .ReturnsDbSet(new List<RoleReadEntity>
                {
                    Helpers.CreateEntity<RoleReadEntity>(new
                    {
                        Id = TestVariables.RoleId,
                        Name = "name",
                        Resources = new List<ResourceReadEntity>
                        {
                            Helpers.CreateEntity<ResourceReadEntity>(new
                            {
                                Id = TestVariables.ResourceId,
                            }),
                        },
                    }),
                });

            var roleQueries = new RoleQueryService(context.Object);
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
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<RoleReadEntity>())
                .ReturnsDbSet(new List<RoleReadEntity>());

            var roleQueries = new RoleQueryService(context.Object);
            var result = await roleQueries.GetSimpleRoles();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetSimpleRoles_GivenDataIsFound_ExpectMaybeWithMappedData()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<RoleReadEntity>())
                .ReturnsDbSet(new List<RoleReadEntity>
                {
                    Helpers.CreateEntity<RoleReadEntity>(new
                    {
                        Id = TestVariables.RoleId,
                        Name= "name",
                    }),
                });

            var roleQueries = new RoleQueryService(context.Object);
            var result = await roleQueries.GetSimpleRoles();
            Assert.True(result.HasValue);
            Assert.Single(result.Value);
            Assert.Equal("name", result.Value.First().Name);
            Assert.Equal(TestVariables.RoleId, result.Value.First().Id);
        }
    }
}