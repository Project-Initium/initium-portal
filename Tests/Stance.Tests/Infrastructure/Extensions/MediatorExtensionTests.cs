﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Stance.Domain.AggregatesModel.RoleAggregate;
using Stance.Infrastructure;
using Stance.Infrastructure.Extensions;
using Xunit;

namespace Stance.Tests.Infrastructure.Extensions
{
    public class MediatorExtensionTests
    {
        [Fact]
        public async Task
            DispatchDomainEventsAsync_GivenEntitiesWithEvents_NotificationsArePublishedAndEventsAreCleared()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();

            await using var context = new DataContext(options, mediator.Object);
            var role = new Role(TestVariables.RoleId, "name", new List<Guid>());
            var @event = new Mock<INotification>();
            role.AddDomainEvent(@event.Object);
            await context.Roles.AddAsync(role);

            await mediator.Object.DispatchDomainEventsAsync(context);

            Assert.Empty(role.DomainEvents);
            mediator.Verify(x => x.Publish(It.IsAny<INotification>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}