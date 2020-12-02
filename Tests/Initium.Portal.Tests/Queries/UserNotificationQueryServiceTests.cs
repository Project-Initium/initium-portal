// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Initium.Portal.Core;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Settings;
using Initium.Portal.Queries;
using Initium.Portal.Queries.Entities;
using MaybeMonad;
using Microsoft.EntityFrameworkCore;
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
            (await context.UserNotifications.AddAsync(new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            })).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.UserNotifications.AddAsync(new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            })).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.UserNotifications.AddAsync(new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            })).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.UserNotifications.AddAsync(new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            })).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.UserNotifications.AddAsync(new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            })).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.UserNotifications.AddAsync(new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            })).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.UserNotifications.AddAsync(new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            })).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.UserNotifications.AddAsync(new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            })).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.UserNotifications.AddAsync(new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            })).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.UserNotifications.AddAsync(new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            })).Property("TenantId").CurrentValue = TestVariables.TenantId;
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
            (await context.UserNotifications.AddAsync(new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
                WhenViewed = TestVariables.Now,
            })).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.UserNotifications.AddAsync(new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
                WhenViewed = TestVariables.Now,
            })).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.UserNotifications.AddAsync(new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
                WhenViewed = TestVariables.Now,
            })).Property("TenantId").CurrentValue = TestVariables.TenantId;
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
            (await context.UserNotifications.AddAsync(new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
                WhenViewed = TestVariables.Now,
            })).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.UserNotifications.AddAsync(new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
            })).Property("TenantId").CurrentValue = TestVariables.TenantId;
            (await context.UserNotifications.AddAsync(new UserNotification
            {
                NotificationId = Guid.NewGuid(),
                UserId = TestVariables.UserId,
                WhenViewed = TestVariables.Now,
            })).Property("TenantId").CurrentValue = TestVariables.TenantId;
            await context.SaveChangesAsync();

            var tenantQueryService = new UserNotificationQueryService(context, currentAuthenticatedUserProvider.Object);
            var result = await tenantQueryService.AnyUnread();
            Assert.True(result);
        }
    }
}