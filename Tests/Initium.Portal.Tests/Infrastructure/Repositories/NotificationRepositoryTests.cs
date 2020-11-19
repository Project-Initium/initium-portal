// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.AggregatesModel.NotificationAggregate;
using Initium.Portal.Infrastructure;
using Initium.Portal.Infrastructure.Admin;
using Initium.Portal.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Infrastructure.Repositories
{
    public class NotificationRepositoryTests
    {
        [Fact]
        public void Add_GivenArgumentIsNotNotificationType_ExpectException()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();
            var tenantInfo = new Mock<FeatureBasedTenantInfo>();

            using var context = new ManagementDataContext(options, mediator.Object, tenantInfo.Object);
            var repository = new NotificationRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => repository.Add(new Mock<Initium.Portal.Domain.AggregatesModel.NotificationAggregate.INotification>().Object));
            Assert.Equal("notification", exception.Message);
        }

        [Fact]
        public void Add_GivenArgumentIsNotificationType_ExpectReturnedNotificationToBeIdenticalAsArgument()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();
            var tenantInfo = new Mock<FeatureBasedTenantInfo>();

            using var context = new ManagementDataContext(options, mediator.Object, tenantInfo.Object);
            var repository = new NotificationRepository(context);

            var notification = new Notification(TestVariables.NotificationId, "subject", "message", NotificationType.AlphaNotification, "serialized-event-data", TestVariables.Now);
            var returned = repository.Add(notification);
            Assert.NotNull(returned);
            Assert.Equal(notification, returned);
        }

        [Fact]
        public void Add_GivenArgumentIsNotificationType_ExpectNotificationToBeAddedToContext()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();
            var tenantInfo = new Mock<FeatureBasedTenantInfo>();

            using var context = new ManagementDataContext(options, mediator.Object, tenantInfo.Object);
            var repository = new NotificationRepository(context);

            var notification = new Notification(TestVariables.NotificationId, "subject", "message", NotificationType.AlphaNotification, "serialized-event-data", TestVariables.Now);
            repository.Add(notification);
            var inContext = context.ChangeTracker.Entries<Notification>()
                .FirstOrDefault(x => x.Entity.Id == TestVariables.NotificationId);
            Assert.NotNull(inContext);
            Assert.Equal(EntityState.Added, inContext.State);
        }
    }
}