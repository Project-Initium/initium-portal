// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MediatR;

namespace Stance.Domain.Events
{
    public class PasswordResetTokenGeneratedEvent : INotification
    {
        public PasswordResetTokenGeneratedEvent(string emailAddress, string token)
        {
            this.EmailAddress = emailAddress;
            this.Token = token;
        }

        public string EmailAddress { get; }

        public string Token { get; }
    }
}