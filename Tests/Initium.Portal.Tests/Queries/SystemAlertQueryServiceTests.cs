// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.MultiTenant;
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
    public class SystemAlertQueryServiceTests
    {
        [Fact]
        public async Task GetActiveSystemAlerts_GivenNoActiveSystemAlerts_ExpectMaybeWithNoData()
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

            await using var context = new ManagementQueryContext(options, tenantInfo, multiTenantSettings.Object);
            context.Add(new SystemAlertReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "name-1",
                Message = "message-1",
                IsActive = false,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new SystemAlertReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "name-2",
                Message = "message-2",
                IsActive = false,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new SystemAlertReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "name-3",
                Message = "message-3",
                IsActive = false,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new SystemAlertReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "name-4",
                Message = "message-4",
                IsActive = false,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            await context.SaveChangesAsync();

            var queries = new SystemAlertQueryService(context);
            var result = await queries.GetActiveSystemAlerts();

            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetActiveSystemAlerts_GivenActiveSystemAlerts_ExpectMaybeWithData()
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

            await using var context = new ManagementQueryContext(options, tenantInfo, multiTenantSettings.Object);
            context.Add(new SystemAlertReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "name-1",
                Message = "message-1",
                IsActive = true,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new SystemAlertReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "name-2",
                Message = "message-2",
                IsActive = false,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new SystemAlertReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "name-3",
                Message = "message-3",
                IsActive = false,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new SystemAlertReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "name-4",
                Message = "message-4",
                IsActive = false,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            await context.SaveChangesAsync();

            var queries = new SystemAlertQueryService(context);
            var result = await queries.GetActiveSystemAlerts();

            Assert.True(result.HasValue);
            Assert.Single(result.Value);
        }

        [Fact]
        public async Task GetDetailedSystemAlertById__GivenNoSystemAlertFound_ExpectMaybeWithNoData()
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

            await using var context = new ManagementQueryContext(options, tenantInfo, multiTenantSettings.Object);
            context.Add(new SystemAlertReadEntity
            {
                Id = Guid.NewGuid(),
                Name = "name",
                Message = "message",
                IsActive = false,
                Type = SystemAlertType.High,
                WhenToHide = TestVariables.Now,
                WhenToShow = TestVariables.Now.AddHours(5),
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;

            await context.SaveChangesAsync();

            var queries = new SystemAlertQueryService(context);
            var result = await queries.GetDetailedSystemAlertById(TestVariables.SystemAlertId);

            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetDetailedSystemAlertById__GivenSystemAlertFound_ExpectMaybeWithCorrectlyMappedData()
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

            await using var context = new ManagementQueryContext(options, tenantInfo, multiTenantSettings.Object);
            context.Add(new SystemAlertReadEntity
            {
                Id = TestVariables.SystemAlertId,
                Name = "name",
                Message = "message",
                IsActive = true,
                Type = SystemAlertType.High,
                WhenToHide = TestVariables.Now,
                WhenToShow = TestVariables.Now.AddHours(5),
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;

            await context.SaveChangesAsync();

            var queries = new SystemAlertQueryService(context);
            var result = await queries.GetDetailedSystemAlertById(TestVariables.SystemAlertId);

            Assert.True(result.HasValue);
            Assert.Equal(TestVariables.SystemAlertId, result.Value.SystemAlertId);
            Assert.Equal("name", result.Value.Name);
            Assert.Equal("message", result.Value.Message);
            Assert.Equal(SystemAlertType.High, result.Value.Type);
            Assert.Equal(TestVariables.Now, result.Value.WhenToHide);
            Assert.Equal(TestVariables.Now.AddHours(5), result.Value.WhenToShow);
        }
    }
}