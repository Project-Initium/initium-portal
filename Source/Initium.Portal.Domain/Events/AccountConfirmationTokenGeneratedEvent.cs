// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MediatR;

namespace Initium.Portal.Domain.Events
{
    public class AccountConfirmationTokenGeneratedEvent : INotification
    {
        public AccountConfirmationTokenGeneratedEvent(string emailAddress, string firstName, string lastName, string token)
        {
            this.Token = token;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.EmailAddress = emailAddress;
        }

        public string EmailAddress { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public string Token { get; }
    }
}