// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Initium.Portal.Queries.Models.User
{
    public class DetailedUserModel
    {
        private readonly List<Guid> _roles;

        public DetailedUserModel(Guid userId, string emailAddress, string firstName, string lastName, bool isLockable, DateTime whenCreated, DateTime? whenLastAuthenticated, DateTime? whenLocked, bool isAdmin, List<Guid> roles, DateTime? whenDisabled)
        {
            this.UserId = userId;
            this.EmailAddress = emailAddress;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.IsLockable = isLockable;
            this.WhenCreated = whenCreated;
            this.WhenLastAuthenticated = whenLastAuthenticated;
            this.WhenLocked = whenLocked;
            this.IsAdmin = isAdmin;
            this._roles = roles;
            this.WhenDisabled = whenDisabled;
        }

        public Guid UserId { get; }

        public string EmailAddress { get; }

        public string FirstName { get; }

        public string LastName { get; }

        public bool IsLockable { get; }

        public DateTime WhenCreated { get; }

        public DateTime? WhenLastAuthenticated { get; }

        public DateTime? WhenLocked { get; }

        public DateTime? WhenDisabled { get; }

        public bool IsAdmin { get; }

        public IReadOnlyList<Guid> Resources => this._roles.AsReadOnly();
    }
}