// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Infrastructure;

namespace Initium.Portal.Queries.Entities
{
    public class UserNotification : ReadEntity
    {
        public Guid NotificationId { get; set; }

        public Guid UserId { get; set; }

        public DateTime WhenNotified { get; set; }

        public NotificationType Type { get; set; }

        public string SerializedEventData { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public DateTime? WhenViewed { get; set; }

        public User User { get; set; }
    }
}