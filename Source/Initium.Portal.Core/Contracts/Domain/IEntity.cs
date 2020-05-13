// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using MediatR;

namespace Initium.Portal.Core.Contracts.Domain
{
    public interface IEntity
    {
        List<INotification> DomainEvents { get; }

        Guid Id { get; }

        void AddDomainEvent(INotification eventItem);

        bool Equals(object obj);

        int GetHashCode();

        bool IsTransient();

        void RemoveDomainEvent(INotification eventItem);
    }
}