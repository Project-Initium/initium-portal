// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.AggregatesModel.NotificationAggregate;

namespace Initium.Portal.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ICoreDataContext _dataContext;

        public NotificationRepository(ICoreDataContext dataContext)
        {
            this._dataContext = dataContext;
        }

        public IUnitOfWork UnitOfWork => this._dataContext;

        public INotification Add(INotification notification)
        {
            var entity = notification as Notification;
            if (entity == null)
            {
                throw new ArgumentException(nameof(notification));
            }

            return this._dataContext.Notifications.Add(entity).Entity;
        }
    }
}