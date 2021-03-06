﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Contracts.Domain;

namespace Initium.Portal.Domain.AggregatesModel.NotificationAggregate
{
    public interface INotificationRepository : IRepository<INotification>
    {
        INotification Add(INotification notification);
    }
}