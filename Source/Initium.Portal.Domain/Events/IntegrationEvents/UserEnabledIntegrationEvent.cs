// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MediatR;

namespace Initium.Portal.Domain.Events.IntegrationEvents
{
    public class UserEnabledIntegrationEvent : INotification
    {
        public UserEnabledIntegrationEvent(string emailAddress, string firstName, string lastName)
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