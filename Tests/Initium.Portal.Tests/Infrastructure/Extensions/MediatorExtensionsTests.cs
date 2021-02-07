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
using Initium.Portal.Infrastructure;
using Initium.Portal.Infrastructure.Admin;
using Initium.Portal.Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();

            var tenantInfo = new Mock<FeatureBasedTenantInfo>();

            await using var context = new GenericDataContext(options, Mock.Of<IServiceProvider>(), Mock.Of<FeatureBasedTenantInfo>());
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
            DDispatchIntegrationEventsAsync_GivenEntitiesWithEvents_NotificationsArePublishedAndEventsAreCleared()
        {
            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();

            var tenantInfo = new Mock<FeatureBasedTenantInfo>();

            await using var context = new GenericDataContext(options, Mock.Of<IServiceProvider>(), Mock.Of<FeatureBasedTenantInfo>());
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