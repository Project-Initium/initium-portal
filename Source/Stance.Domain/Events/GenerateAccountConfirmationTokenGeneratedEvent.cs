// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MediatR;

namespace Stance.Domain.Events
{
    public class GenerateAccountConfirmationTokenGeneratedEvent : INotification
    {
        public GenerateAccountConfirmationTokenGeneratedEvent(string token, string emailAddress)
        {
            this.Token = token;
            this.EmailAddress = emailAddress;
        }

        public string Token { get; }

        public string EmailAddress { get; }
    }
}