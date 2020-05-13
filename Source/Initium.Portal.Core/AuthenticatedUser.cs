// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Core
{
    public sealed class AuthenticatedUser : ISystemUser
    {
        public AuthenticatedUser(Guid userId, string emailAddress, string firstName, string lastName)
        {
            this.UserId = userId;
            this.EmailAddress = emailAddress;
            this.FirstName = firstName;
            this.LastName = lastName;
        }

        public Guid UserId { get; }

        public string EmailAddress { get; }

        public string FirstName { get; }

        public string LastName { get; }
    }
}