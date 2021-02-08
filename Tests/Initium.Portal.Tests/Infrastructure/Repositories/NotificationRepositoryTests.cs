// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.AggregatesModel.NotificationAggregate;
using Initium.Portal.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Infrastructure.Repositories
{
    public class NotificationRepositoryTests
    {
        [Fact]
        public void UnitOfWork_GivenDataContextSetInConstructor_ExpectSameValue()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>()
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var repository = new NotificationRepository(context);
            Assert.Equal(context, repository.UnitOfWork);
        }

        [Fact]
        public void Add_GivenArgumentIsNotNotificationType_ExpectException()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>()
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var repository = new NotificationRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => repository.Add(new Mock<Initium.Portal.Domain.AggregatesModel.NotificationAggregate.INotification>().Object));
            Assert.Equal("notification", exception.ParamName);
        }

        [Fact]
        public void Add_GivenArgumentIsNotificationType_ExpectReturnedNotificationToBeIdenticalAsArgument()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>()
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var repository = new NotificationRepository(context);

            var notification = new Notification(TestVariables.NotificationId, "subject", "message", NotificationType.AlphaNotification, "serialized-event-data", TestVariables.Now);
            var returned = repository.Add(notification);
            Assert.NotNull(returned);
            Assert.Equal(notification, returned);
        }

        [Fact]
        public void Add_GivenArgumentIsNotificationType_ExpectNotificationToBeAddedToContext()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>()
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
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