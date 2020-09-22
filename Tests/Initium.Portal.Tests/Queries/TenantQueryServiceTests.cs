// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Initium.Portal.Core.Settings;
using Initium.Portal.Queries;
using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Queries
{
    public class TenantQueryServiceTests
    {
        [Fact]
        public async Task CheckForPresenceOfTenantByIdentifier_GivenTenantDoesNotExist_ExpectNotPresentStatus()
        {
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new Mock<ITenantInfo>();
            tenantInfo.Setup(x => x.Id).Returns(TestVariables.TenantId.ToString);

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.DefaultTenantId,
            });

            await using var context = new QueryContext(options, tenantInfo.Object, multiTenantSettings.Object);
            var tenantQueryService = new TenantQueryService(context);
            var result = await tenantQueryService.CheckForPresenceOfTenantByIdentifier("identifier");
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfTenantByIdentifier_GivenTenantDoesExist_ExpectPresentStatus()
        {
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new Mock<ITenantInfo>();
            tenantInfo.Setup(x => x.Id).Returns(TestVariables.TenantId.ToString);

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.DefaultTenantId,
            });

            await using var context = new QueryContext(options, tenantInfo.Object, multiTenantSettings.Object);
            await context.AddAsync(new TenantDto
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
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new Mock<ITenantInfo>();
            tenantInfo.Setup(x => x.Id).Returns(TestVariables.TenantId.ToString);

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.DefaultTenantId,
            });

            await using var context = new QueryContext(options, tenantInfo.Object, multiTenantSettings.Object);
            var tenantQueryService = new TenantQueryService(context);
            var maybe = await tenantQueryService.GetTenantMetadataById(TestVariables.TenantId);
            Assert.True(maybe.HasNoValue);
        }

        [Fact]
        public async Task GetTenantMetadataByIdr_GivenTenantDoesExist_ExpectMaybeWithData()
        {
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new Mock<ITenantInfo>();
            tenantInfo.Setup(x => x.Id).Returns(TestVariables.TenantId.ToString);

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.DefaultTenantId,
            });

            await using var context = new QueryContext(options, tenantInfo.Object, multiTenantSettings.Object);
            await context.AddAsync(new TenantDto
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