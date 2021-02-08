// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Initium.Portal.Core.Authentication;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Core.Settings;
using Initium.Portal.Queries;
using Initium.Portal.Queries.Entities;
using Initium.Portal.Queries.Management;
using MaybeMonad;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Queries
{
    public class UserNotificationQueryServiceTests
    {
        [Fact]
        public async Task GetLatestNotifications_GivenNoUserIsAuthenticated_ExpectMaybeWithNoData()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
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
            var tenantQueryService = new UserNotificationQueryService(context, currentAuthenticatedUserProvider.Object);
            var maybe = await tenantQueryService.GetLatestNotifications(1);
            Assert.True(maybe.HasNoValue);
        }

        [Theory]
        [InlineData(2)]
        [InlineData(5)]
        [InlineData(8)]
        public async Task GetLatestNotifications_GivenUserIsAuthenticated_ExpectMaybeWithData(int top)
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe.From(new AuthenticatedUser(
                TestVariables.UserId,
                "email-address",
                "first-name",
                "last-name") as ISystemUser));

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
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
            (await context.Set<UserNotificationReadEntity>().AddAsync(
                Helpers.CreateEntity<UserNotificationReadEntity>(new
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            }))).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.Set<UserNotificationReadEntity>().AddAsync(
                Helpers.CreateEntity<UserNotificationReadEntity>(new
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            }))).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.Set<UserNotificationReadEntity>().AddAsync(
                Helpers.CreateEntity<UserNotificationReadEntity>(new
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            }))).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.Set<UserNotificationReadEntity>().AddAsync(
                Helpers.CreateEntity<UserNotificationReadEntity>(new
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            }))).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.Set<UserNotificationReadEntity>().AddAsync(
                Helpers.CreateEntity<UserNotificationReadEntity>(new
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            }))).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.Set<UserNotificationReadEntity>().AddAsync(
                Helpers.CreateEntity<UserNotificationReadEntity>(new
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            }))).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.Set<UserNotificationReadEntity>().AddAsync(
                Helpers.CreateEntity<UserNotificationReadEntity>(new
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            }))).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.Set<UserNotificationReadEntity>().AddAsync(
                Helpers.CreateEntity<UserNotificationReadEntity>(new
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            }))).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.Set<UserNotificationReadEntity>().AddAsync(
                Helpers.CreateEntity<UserNotificationReadEntity>(new
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            }))).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.Set<UserNotificationReadEntity>().AddAsync(
                Helpers.CreateEntity<UserNotificationReadEntity>(new
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            }))).Property("TenantId").CurrentValue = TestVariables.TenantId;
            await context.SaveChangesAsync();

            var tenantQueryService = new UserNotificationQueryService(context, currentAuthenticatedUserProvider.Object);
            var maybe = await tenantQueryService.GetLatestNotifications(top);
            Assert.True(maybe.HasValue);
            Assert.Equal(top, maybe.Value.Count);
        }

        [Fact]
        public async Task AnyUnread_GivenNoUserIsAuthenticated_ExpectFalse()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
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
            var tenantQueryService = new UserNotificationQueryService(context, currentAuthenticatedUserProvider.Object);
            var result = await tenantQueryService.AnyUnread();
            Assert.False(result);
        }

        [Fact]
        public async Task AnyUnread_GivenUserHasNoNotifications_ExpectFalse()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe.From(new AuthenticatedUser(
                TestVariables.UserId,
                "email-address",
                "first-name",
                "last-name") as ISystemUser));

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
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
            var tenantQueryService = new UserNotificationQueryService(context, currentAuthenticatedUserProvider.Object);
            var result = await tenantQueryService.AnyUnread();
            Assert.False(result);
        }

        [Fact]
        public async Task AnyUnread_GivenUserHasNotificationsButAllAreViewed_ExpectFalse()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe.From(new AuthenticatedUser(
                TestVariables.UserId,
                "email-address",
                "first-name",
                "last-name") as ISystemUser));

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
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
            (await context.Set<UserNotificationReadEntity>().AddAsync(
                Helpers.CreateEntity<UserNotificationReadEntity>(new
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
                WhenViewed = TestVariables.Now,
            }))).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.Set<UserNotificationReadEntity>().AddAsync(
                Helpers.CreateEntity<UserNotificationReadEntity>(new
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
                WhenViewed = TestVariables.Now,
            }))).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.Set<UserNotificationReadEntity>().AddAsync(
                Helpers.CreateEntity<UserNotificationReadEntity>(new
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
                WhenViewed = TestVariables.Now,
            }))).Property("TenantId").CurrentValue = TestVariables.TenantId;
            await context.SaveChangesAsync();

            var tenantQueryService = new UserNotificationQueryService(context, currentAuthenticatedUserProvider.Object);
            var result = await tenantQueryService.AnyUnread();
            Assert.False(result);
        }

        [Fact]
        public async Task AnyUnread_GivenUserHasNotificationsButAllAreNotViewed_ExpectTrue()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe.From(new AuthenticatedUser(
                TestVariables.UserId,
                "email-address",
                "first-name",
                "last-name") as ISystemUser));

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
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
            (await context.Set<UserNotificationReadEntity>().AddAsync(
                Helpers.CreateEntity<UserNotificationReadEntity>(new
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
                WhenViewed = TestVariables.Now,
            }))).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.Set<UserNotificationReadEntity>().AddAsync(
                Helpers.CreateEntity<UserNotificationReadEntity>(new
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            }))).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.Set<UserNotificationReadEntity>().AddAsync(
                Helpers.CreateEntity<UserNotificationReadEntity>(new
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
                WhenViewed = TestVariables.Now,
            }))).Property("TenantId").CurrentValue = TestVariables.TenantId;
            await context.SaveChangesAsync();

            var tenantQueryService = new UserNotificationQueryService(context, currentAuthenticatedUserProvider.Object);
            var result = await tenantQueryService.AnyUnread();
            Assert.True(result);
        }
    }
}