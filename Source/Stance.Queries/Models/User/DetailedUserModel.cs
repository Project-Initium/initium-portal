// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Stance.Queries.Models.User
{
    public class DetailedUserModel
    {
        public DetailedUserModel(Guid userId, string emailAddress, string firstName, string lastName, bool isLockable, DateTime whenCreated, DateTime? whenLastAuthenticated, DateTime? whenLocked)
        {
            this.UserId = userId;
            this.EmailAddress = emailAddress;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.IsLockable = isLockable;
            this.WhenCreated = whenCreated;
            this.WhenLastAuthenticated = whenLastAuthenticated;
            this.WhenLocked = whenLocked;
        }

        public Guid UserId { get; }

        public string EmailAddress { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public bool IsLockable { get; }

        public DateTime WhenCreated { get; }

        public DateTime? WhenLastAuthenticated { get; }

        public DateTime? WhenLocked { get; }
    }
}