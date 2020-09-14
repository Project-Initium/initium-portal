// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Infrastructure;

namespace Initium.Portal.Queries.Entities
{
    public class User : ReadEntity
    {
        public User()
        {
            this.UserRoles = new List<UserRole>();
            this.AuthenticatorApps = new List<AuthenticatorApp>();
            this.AuthenticatorDevices = new List<AuthenticatorDevice>();
            this.UserNotifications = new List<UserNotification>();
        }

        public Guid Id { get; set; }

        public string EmailAddress { get; set; }

        public bool IsLockable { get; set; }

        public bool IsLocked { get; set; }

        public DateTime? WhenLastAuthenticated { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime WhenCreated { get; set; }

        public bool IsVerified { get; set; }

        public bool IsAdmin { get; set; }

        public DateTime? WhenLocked { get; set; }

        public DateTime? WhenDisabled { get; set; }

        public List<UserRole> UserRoles { get; set; }

        public List<AuthenticatorApp> AuthenticatorApps { get; set; }

        public List<AuthenticatorDevice> AuthenticatorDevices { get; set; }

        public List<UserNotification> UserNotifications { get; set; }
    }
}