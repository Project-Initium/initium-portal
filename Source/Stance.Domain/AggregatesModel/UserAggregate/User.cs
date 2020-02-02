// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Core.Domain;

namespace Stance.Domain.AggregatesModel.UserAggregate
{
    public sealed class User : Entity, IUser
    {
        public User(Guid id, string emailAddress, string passwordHash, DateTime whenCreated)
        {
            this.Id = id;
            this.EmailAddress = emailAddress;
            this.PasswordHash = passwordHash;
            this.WhenCreated = whenCreated;
        }

        private User()
        {
        }

        public string EmailAddress { get; private set; }

        public string PasswordHash { get; private set; }

        public DateTime WhenCreated { get; private set; }
    }
}