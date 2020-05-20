// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Domain;

namespace Initium.Portal.Domain.AggregatesModel.UserAggregate
{
    public sealed class UserNotification : Entity
    {
#pragma warning disable 628
        protected internal UserNotification(Guid id)
#pragma warning restore 628
        {
            this.Id = id;
        }

        private UserNotification()
        {
        }

        public DateTime? WhenViewed { get; private set; }

        public DateTime? WhenDismissed { get; private set; }

        public Guid NotificationId => this.Id;

        public void MarkAsViewed(DateTime whenViewed)
        {
            this.WhenViewed = whenViewed;
        }

        public void MarkAsDismissed(DateTime whenDismissed)
        {
            this.WhenDismissed = whenDismissed;
        }
    }
}