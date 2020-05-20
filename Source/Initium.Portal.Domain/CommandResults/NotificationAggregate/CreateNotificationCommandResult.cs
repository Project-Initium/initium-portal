// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Domain.CommandResults.NotificationAggregate
{
    public class CreateNotificationCommandResult
    {
        public CreateNotificationCommandResult(Guid notificationId)
        {
            this.NotificationId = notificationId;
        }

        public Guid NotificationId { get; }
    }
}