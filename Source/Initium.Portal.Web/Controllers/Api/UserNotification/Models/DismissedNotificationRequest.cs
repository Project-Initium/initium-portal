// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using FluentValidation;

namespace Initium.Portal.Web.Controllers.Api.UserNotification.Models
{
    public class DismissedNotificationRequest
    {
        public Guid NotificationId { get; set; }

        public class Validator : AbstractValidator<ViewedNotificationRequest>
        {
            public Validator()
            {
                this.RuleFor(x => x.NotificationId)
                    .NotEqual(Guid.Empty);
            }
        }
    }
}