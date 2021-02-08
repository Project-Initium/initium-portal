// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Core.Settings;
using Initium.Portal.Queries;
using Initium.Portal.Queries.Management;
using Initium.Portal.Queries.Management.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Queries_Management
{
    public class TenantQueryServiceTests
    {
        [Fact]
        public async Task CheckForPresenceOfTenantByIdentifier_GivenTenantDoesNotExist_ExpectNotPresentStatus()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddManagementCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>()
                .Options;

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.DefaultTenantId,
            });

            await using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var tenantQueryService = new TenantQueryService(context);
            var result = await tenantQueryService.CheckForPresenceOfTenantByIdentifier("identifier");
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfTenantByIdentifier_GivenTenantDoesExist_ExpectPresentStatus()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddManagementCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>()
                .Options;

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.DefaultTenantId,
            });

            await using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            await context.AddAsync(new TenantReadEntity
            {
                Id = TestVariables.TenantId,
                Identifier = "identifier",
            });
            await context.SaveChangesAsync();

            var tenantQueryService = new TenantQueryService(context);
            var result = await tenantQueryService.CheckForPresenceOfTenantByIdentifier("identifier");
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task GetTenantMetadataByIdr_GivenTenantDoesNotExist_ExpectMaybeWithoutData()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddManagementCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>()
                .Options;

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.DefaultTenantId,
            });

            await using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var tenantQueryService = new TenantQueryService(context);
            var maybe = await tenantQueryService.GetTenantMetadataById(TestVariables.TenantId);
            Assert.True(maybe.HasNoValue);
        }

        [Fact]
        public async Task GetTenantMetadataByIdr_GivenTenantDoesExist_ExpectMaybeWithData()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddManagementCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>()
                .Options;

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.DefaultTenantId,
            });

            await using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            await context.AddAsync(new TenantReadEntity
            {
                Id = TestVariables.TenantId,
                Identifier = "identifier",
            });
            await context.SaveChangesAsync();
            var tenantQueryService = new TenantQueryService(context);

            var maybe = await tenantQueryService.GetTenantMetadataById(TestVariables.TenantId);
            Assert.True(maybe.HasValue);
        }
    }
}