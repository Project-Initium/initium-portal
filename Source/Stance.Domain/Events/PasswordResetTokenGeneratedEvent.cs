// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MediatR;

namespace Stance.Domain.Events
{
    public class PasswordResetTokenGeneratedEvent : INotification
    {
        public PasswordResetTokenGeneratedEvent(string emailAddress, string firstName, string lastName, string token)
        {
            this.EmailAddress = emailAddress;
            this.Token = token;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public string EmailAddress { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Token { get; }
    }
}