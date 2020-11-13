// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using Initium.Portal.Core.Domain;
using MediatR;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Core.Domain
{
    [SuppressMessage("ReSharper", "EqualExpressionComparison", Justification = "Needed for unit tests")]
    [SuppressMessage("ReSharper", "S1764", Justification = "Needed for unit tests")]
    public class EntityTests
    {
#pragma warning disable CS1718
        [Fact]
        public void EqualOperator_GivenEntitiesAreTheSame_ExpectTrue()
        {
            var entity = new DummyEntity();

            Assert.True(entity == entity);
        }
#pragma warning restore CS1718

        [Fact]
        public void EqualOperator_GivenEntitiesAreNotTheSame_ExpectFalse()
        {
            var entity1 = new DummyEntity();
            var entity2 = new DummyEntity();
            Assert.False(entity1 == entity2);
        }

#pragma warning disable CS1718

        [Fact]
        public void NotEqualOperator_GivenEntitiesAreTheSame_ExpectFalse()
        {
            var entity = new DummyEntity();

            Assert.False(entity != entity);
        }

#pragma warning restore CS1718

        [Fact]
        public void NotEqualOperator_GivenEntitiesAreNotTheSame_ExpectTrue()
        {
            var entity1 = new DummyEntity();
            var entity2 = new DummyEntity();
            Assert.True(entity1 != entity2);
        }

        [Fact]
        public void AddDomainEvent_GivenArgumentIsPassedAndNoItemInList_ExpectListWithSingleItem()
        {
            var entity = new DummyEntity();
            entity.AddDomainEvent(Mock.Of<INotification>());

            Assert.Single(entity.DomainEvents);
        }

        [Fact]
        public void AddDomainEvent_GivenArgumentIsPassedAndItemInList_ExpectListWithTwoItems()
        {
            var entity = new DummyEntity();
            entity.AddDomainEvent(Mock.Of<INotification>());
            entity.AddDomainEvent(Mock.Of<INotification>());

            Assert.Equal(2, entity.DomainEvents.Count);
        }

        [Fact]
        public void AddIntegrationEvent_GivenArgumentIsPassedAndNoItemInList_ExpectListWithSingleItem()
        {
            var entity = new DummyEntity();
            entity.AddIntegrationEvent(Mock.Of<INotification>());

            Assert.Single(entity.IntegrationEvents);
        }

        [Fact]
        public void AddIntegrationEvent_GivenArgumentIsPassedAndItemInList_ExpectListWithTwoItems()
        {
            var entity = new DummyEntity();
            entity.AddIntegrationEvent(Mock.Of<INotification>());
            entity.AddIntegrationEvent(Mock.Of<INotification>());

            Assert.Equal(2, entity.IntegrationEvents.Count);
        }

        [Fact]
        public void RemoveDomainEvent_GivenArgumentIsPassed_ExpectItemRemoved()
        {
            var entity = new DummyEntity();
            var notification = Mock.Of<INotification>();
            entity.AddDomainEvent(notification);
            entity.RemoveDomainEvent(notification);

            Assert.Empty(entity.DomainEvents);
        }

        [Fact]
        public void RemoveIntegrationEvent_GivenArgumentIsPassed_ExpectItemRemoved()
        {
            var entity = new DummyEntity();
            var notification = Mock.Of<INotification>();
            entity.AddIntegrationEvent(notification);
            entity.RemoveIntegrationEvent(notification);

            Assert.Empty(entity.IntegrationEvents);
        }

        [Fact]
        public void IsTransient_GivenIdIsNotSet_ExpectTrue()
        {
            var entity = new DummyEntity();

            Assert.True(entity.IsTransient());
        }

        [Fact]
        public void IsTransient_GivenIdIsSet_ExpectFalse()
        {
            var entity = new DummyEntity(Guid.NewGuid());

            Assert.False(entity.IsTransient());
        }

        private sealed class DummyEntity : Entity
        {
            public DummyEntity()
            {
            }

            public DummyEntity(Guid id)
            {
                this.Id = id;
            }
        }
    }
}