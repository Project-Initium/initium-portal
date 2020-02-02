// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using MediatR;
using Stance.Core.Contracts.Domain;

namespace Stance.Core.Domain
{
    public abstract class Entity : IEntity
    {
        private int? _requestedHashCode;

        public virtual Guid Id { get; protected set; }

        public List<INotification> DomainEvents { get; private set; }

        public static bool operator ==(Entity left, Entity right)
        {
            return left?.Equals(right) ?? Equals(right, null);
        }

        public static bool operator !=(Entity left, Entity right)
        {
            return !(left == right);
        }

        public void AddDomainEvent(INotification eventItem)
        {
            this.DomainEvents = this.DomainEvents ?? new List<INotification>();
            this.DomainEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            this.DomainEvents?.Remove(eventItem);
        }

        public bool IsTransient()
        {
            return this.Id == default;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Entity))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (this.GetType() != obj.GetType())
            {
                return false;
            }

            var item = (Entity)obj;

            if (item.IsTransient() || this.IsTransient())
            {
                return false;
            }

            return item.Id == this.Id;
        }

        public override int GetHashCode()
        {
            if (!this.IsTransient())
            {
                if (!this._requestedHashCode.HasValue)
                {
                    this._requestedHashCode = this.Id.GetHashCode() ^ 31;
                }

                return this._requestedHashCode.Value;
            }

            return base.GetHashCode();
        }
    }
}