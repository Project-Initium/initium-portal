// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Contracts.Domain;
using MediatR;

namespace Initium.Portal.Core.Domain
{
    public abstract class Entity : IEntity
    {
        private List<INotification> _integrationEvents;
        private List<INotification> _domainEvents;
        private int? _requestedHashCode;

        public virtual Guid Id { get; protected set; }

        public IReadOnlyCollection<INotification> IntegrationEvents => this._integrationEvents?.AsReadOnly();

        public IReadOnlyCollection<INotification> DomainEvents => this._domainEvents?.AsReadOnly();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Blocker Code Smell", "S3875:\"operator==\" should not be overloaded on reference types", Justification = "Initially suppressing everything before evaluating what's wanted and what's appropriate.")]
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
            this._domainEvents = this._domainEvents ?? new List<INotification>();
            this._domainEvents.Add(eventItem);
        }

        public void AddIntegrationEvent(INotification eventItem)
        {
            this._integrationEvents = this._integrationEvents ?? new List<INotification>();
            this._integrationEvents.Add(eventItem);
        }

        public void RemoveDomainEvent(INotification eventItem)
        {
            this._domainEvents?.Remove(eventItem);
        }

        public void RemoveIntegrationEvent(INotification eventItem)
        {
            this._integrationEvents?.Remove(eventItem);
        }

        public void ClearDomainEvents()
        {
            this._domainEvents?.Clear();
        }

        public void ClearIntegrationEvents()
        {
            this._integrationEvents?.Clear();
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Bug", "S2328:\"GetHashCode\" should not reference mutable fields", Justification = "Initially suppressing everything before evaluating what's wanted and what's appropriate.")]
        public override int GetHashCode()
        {
            if (this.IsTransient())
            {
                return base.GetHashCode();
            }

            if (!this._requestedHashCode.HasValue)
            {
                this._requestedHashCode = this.Id.GetHashCode() ^ 31;
            }

            return this._requestedHashCode.Value;
        }
    }
}