// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Constants;

namespace Initium.Portal.Queries.Models.Notifications
{
    public class SimpleNotification
    {
        public SimpleNotification(string subject, string message, NotificationType type, string serializedEventData, bool isRead)
        {
            this.Subject = subject;
            this.Message = message;
            this.Type = type;
            this.SerializedEventData = serializedEventData;
            this.IsRead = isRead;
        }

        public string Subject { get; }

        public string Message { get; }

        public NotificationType Type { get; }

        public string SerializedEventData { get; }

        public bool IsRead { get; }
    }
}