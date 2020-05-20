// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Domain;

namespace Initium.Portal.Domain.AggregatesModel.NotificationAggregate
{
    public sealed class Notification : Entity, INotification
    {
        private readonly List<UserNotification> _userNotifications;

        public Notification(Guid id, string subject, string message, NotificationType type, string serializedEventData, DateTime whenNotified)
        {
            this.Id = id;
            this.Subject = subject;
            this.Message = message;
            this.Type = type;
            this.SerializedEventData = serializedEventData;
            this.WhenNotified = whenNotified;
            this._userNotifications = new List<UserNotification>();
        }

        private Notification()
        {
            this._userNotifications = new List<UserNotification>();
        }

        public string Subject { get; private set; }

        public string Message { get; private set; }

        public NotificationType Type { get; private set; }

        public string SerializedEventData { get; }

        public DateTime WhenNotified { get; private set; }

        public IReadOnlyList<UserNotification> UserNotifications => this._userNotifications.AsReadOnly();

        public void SendToUser(Guid userId)
        {
            this._userNotifications.Add(new UserNotification(userId));
        }
    }
}