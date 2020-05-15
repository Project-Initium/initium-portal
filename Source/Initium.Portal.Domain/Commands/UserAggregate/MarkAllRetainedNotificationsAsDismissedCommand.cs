// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Domain;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.UserAggregate
{
    public class MarkAllRetainedNotificationsAsDismissedCommand : IRequest<ResultWithError<ErrorData>>
    {
        public MarkAllRetainedNotificationsAsDismissedCommand(Guid userId)
        {
            this.UserId = userId;
        }

        public Guid UserId { get; }
    }
}