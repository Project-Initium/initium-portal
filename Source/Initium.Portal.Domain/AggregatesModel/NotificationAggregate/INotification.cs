// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts.Domain;

namespace Initium.Portal.Domain.AggregatesModel.NotificationAggregate
{
    public interface INotification : IAggregateRoot, IEntity
    {
        string Subject { get; }

        string Message { get; }

        NotificationType Type { get; }

        string SerializedEventData { get; }

        DateTime WhenNotified { get; }

        IReadOnlyList<UserNotification> UserNotifications { get; }

        void SendToUser(Guid userId);
    }
}