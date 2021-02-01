// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Infrastructure;

namespace Initium.Portal.Queries.Entities
{
    public class UserReadEntity : ReadEntity
    {
        public UserReadEntity()
        {
            this.UserRoles = new List<UserRoleReadEntity>();
            this.AuthenticatorApps = new List<AuthenticatorAppReadEntity>();
            this.AuthenticatorDevices = new List<AuthenticatorDeviceReadEntity>();
            this.UserNotifications = new List<UserNotificationReadEntity>();
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

        public List<UserRoleReadEntity> UserRoles { get; set; }

        public List<AuthenticatorAppReadEntity> AuthenticatorApps { get; set; }

        public List<AuthenticatorDeviceReadEntity> AuthenticatorDevices { get; set; }

        public List<UserNotificationReadEntity> UserNotifications { get; set; }
    }
}