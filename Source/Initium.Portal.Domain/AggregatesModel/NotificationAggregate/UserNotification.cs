// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Domain;

namespace Initium.Portal.Domain.AggregatesModel.NotificationAggregate
{
    public sealed class UserNotification : Entity
    {
        public UserNotification(Guid userId)
        {
            this.Id = userId;
        }

        private UserNotification()
        {
        }
    }
}