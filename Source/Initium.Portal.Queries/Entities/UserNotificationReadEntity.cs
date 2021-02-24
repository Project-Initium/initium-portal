// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts.Queries;

namespace Initium.Portal.Queries.Entities
{
    public class UserNotificationReadEntity : ReadOnlyEntity
    {
        public Guid NotificationId { get; private set; }

        public Guid UserId { get; private set; }

        public DateTime WhenNotified { get; private set; }

        public NotificationType Type { get; private set; }

        public string SerializedEventData { get; private set; }

        public string Subject { get; private set; }

        public string Message { get; private set; }

        public DateTime? WhenViewed { get; private set; }

        public UserReadEntity User { get; private set; }
    }
}