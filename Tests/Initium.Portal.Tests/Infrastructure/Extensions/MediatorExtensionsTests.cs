// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.Extensions;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Infrastructure.Extensions
{
    public class MediatorExtensionsTests
    {
        [Fact]
        public async Task
            DispatchDomainEventsAsync_GivenEntitiesWithEvents_NotificationsArePublishedAndEventsAreCleared()
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

            var mediator = new Mock<IMediator>();

            await using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>(), Mock.Of<IMediator>());
            var role = new Role(TestVariables.RoleId, "name", new List<Guid>());
            var @event = new Mock<INotification>();
            role.AddDomainEvent(@event.Object);
            await context.Set<Role>().AddAsync(role);

            await mediator.Object.DispatchDomainEventsAsync(context);

            Assert.Empty(role.DomainEvents);
            mediator.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task
            DispatchIntegrationEventsAsync_GivenEntitiesWithEvents_NotificationsArePublishedAndEventsAreCleared()
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

            var mediator = new Mock<IMediator>();

            await using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>(), Mock.Of<IMediator>());
            var role = new Role(TestVariables.RoleId, "name", new List<Guid>());
            var @event = new Mock<INotification>();
            role.AddIntegrationEvent(@event.Object);
            await context.Set<Role>().AddAsync(role);

            await mediator.Object.DispatchIntegrationEventsAsync(context);

            Assert.Empty(role.IntegrationEvents);
            mediator.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}