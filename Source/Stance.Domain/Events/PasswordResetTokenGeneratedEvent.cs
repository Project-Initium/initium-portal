// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using MediatR;

namespace Stance.Domain.Events
{
    public class PasswordResetTokenGeneratedEvent : INotification
    {
        public PasswordResetTokenGeneratedEvent(Guid userId, string emailAddress, string token)
        {
            this.UserId = userId;
            this.EmailAddress = emailAddress;
            this.Token = token;
        }

        public Guid UserId { get; }

        public string EmailAddress { get; }

        public string Token { get; }
    }
}