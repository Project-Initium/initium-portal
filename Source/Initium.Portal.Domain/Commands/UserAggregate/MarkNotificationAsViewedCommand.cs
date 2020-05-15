// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Domain;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.UserAggregate
{
    public class MarkNotificationAsViewedCommand : IRequest<ResultWithError<ErrorData>>
    {
        public MarkNotificationAsViewedCommand(Guid userId, Guid notificationId)
        {
            this.UserId = userId;
            this.NotificationId = notificationId;
        }

        public Guid UserId { get; }

        public Guid NotificationId { get; }
    }
}