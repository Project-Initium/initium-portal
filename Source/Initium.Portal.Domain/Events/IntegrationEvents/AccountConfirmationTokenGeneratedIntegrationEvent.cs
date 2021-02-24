// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using MediatR;

namespace Initium.Portal.Domain.Events.IntegrationEvents
{
    public class AccountConfirmationTokenGeneratedIntegrationEvent : INotification
    {
        public AccountConfirmationTokenGeneratedIntegrationEvent(string emailAddress, string firstName, string lastName, Guid token, DateTime whenExpires)
        {
            this.FirstName = firstName;
            this.LastName = lastName;
            this.Token = token;
            this.WhenExpires = whenExpires;
            this.EmailAddress = emailAddress;
        }

        public string EmailAddress { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public Guid Token { get; }

        public DateTime WhenExpires { get; }
    }
}