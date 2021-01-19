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
            this.UserNotifications = new List<UserNotification>();
        }

        public Guid Id { get; private set; }

        public string EmailAddress { get; private set; }

        public bool IsLockable { get; private set; }

        public bool IsLocked { get; private set; }

        public DateTime? WhenLastAuthenticated { get; private set; }

        public string FirstName { get; private set; }

        public string LastName { get; private set; }

        public DateTime WhenCreated { get; private set; }

        public bool IsVerified { get; private set; }

        public bool IsAdmin { get; private set; }

        public DateTime? WhenLocked { get; private set; }

        public DateTime? WhenDisabled { get; private set; }

        public List<UserRoleReadEntity> UserRoles { get; set; }

        public List<AuthenticatorAppReadEntity> AuthenticatorApps { get; set; }

        public List<AuthenticatorDeviceReadEntity> AuthenticatorDevices { get; set; }

        public List<UserNotification> UserNotifications { get; set; }
    }
}