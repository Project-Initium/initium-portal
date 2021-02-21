// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Core.Settings;
using Initium.Portal.Queries.Management;
using Initium.Portal.Queries.Management.Entities;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Moq.EntityFrameworkCore;
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

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.DefaultTenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<TenantReadEntity>())
                .ReturnsDbSet(new List<TenantReadEntity>());

            var tenantQueryService = new TenantQueryService(context.Object);
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

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.DefaultTenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<TenantReadEntity>())
                .ReturnsDbSet(new List<TenantReadEntity>
                {
                    Helpers.CreateEntity<TenantReadEntity>(new
                    {
                        Id = TestVariables.TenantId,
                        Identifier = "identifier",
                    }),
                });

            var tenantQueryService = new TenantQueryService(context.Object);
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

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.DefaultTenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<TenantReadEntity>())
                .ReturnsDbSet(new List<TenantReadEntity>());
            var tenantQueryService = new TenantQueryService(context.Object);
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

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.DefaultTenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<TenantReadEntity>())
                .ReturnsDbSet(new List<TenantReadEntity>
                {
                    Helpers.CreateEntity<TenantReadEntity>(new
                    {
                        Id = TestVariables.TenantId,
                        Identifier = "identifier",
                    }),
                });

            var tenantQueryService = new TenantQueryService(context.Object);

            var maybe = await tenantQueryService.GetTenantMetadataById(TestVariables.TenantId);
            Assert.True(maybe.HasValue);
        }
    }
}