// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.CommandResults.NotificationAggregate;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.NotificationAggregate
{
    public class CreateNotificationCommand : IRequest<Result<CreateNotificationCommandResult, ErrorData>>
    {
        public CreateNotificationCommand(string subject, string message, NotificationType type,
            string serializedEventData, DateTime whenNotified, List<Guid> userIds)
        {
            this.UserIds = userIds;
            this.Subject = subject;
            this.Message = message;
            this.Type = type;
            this.SerializedEventData = serializedEventData;
            this.WhenNotified = whenNotified;
        }

        public string Subject { get; }

        public string Message { get; }

        public NotificationType Type { get; }

        public string SerializedEventData { get; }

        public DateTime WhenNotified { get; }

        public IReadOnlyList<Guid> UserIds { get; }
    }
}