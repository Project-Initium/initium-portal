// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Domain;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.UserAggregate
{
    public class MarkNotificationAsDismissedCommand : IRequest<ResultWithError<ErrorData>>
    {
        public MarkNotificationAsDismissedCommand(Guid userId, Guid notificationId)
        {
            this.UserId = userId;
            this.NotificationId = notificationId;
        }

        public Guid UserId { get; }

        public Guid NotificationId { get; }
    }
}