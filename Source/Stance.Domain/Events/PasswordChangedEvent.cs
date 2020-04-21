// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MediatR;

namespace Stance.Domain.Events
{
    public class PasswordChangedEvent : INotification
    {
        public PasswordChangedEvent(string emailAddress, string firstName, string lastName)
        {
            this.EmailAddress = emailAddress;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public string EmailAddress { get; }

        public string FirstName { get; }

        public string LastName { get; }
    }
}