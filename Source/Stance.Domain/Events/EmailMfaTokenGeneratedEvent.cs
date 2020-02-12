// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using MediatR;

namespace Stance.Domain.Events
{
    public class EmailMfaTokenGeneratedEvent : INotification
    {
        public EmailMfaTokenGeneratedEvent(Guid userId, string userEmailAddress, string generated)
        {
            this.UserId = userId;
            this.UserEmailAddress = userEmailAddress;
            this.Generated = generated;
        }

        public Guid UserId { get; }

        public string UserEmailAddress { get; }

        public string Generated { get; }
    }
}