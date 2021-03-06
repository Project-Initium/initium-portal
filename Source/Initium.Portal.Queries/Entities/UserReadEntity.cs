﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Contracts.Queries;

namespace Initium.Portal.Queries.Entities
{
    public class UserReadEntity : ReadOnlyEntity
    {
        private readonly List<RoleReadEntity> _roles;
        private readonly List<AuthenticatorAppReadEntity> _authenticatorApps;
        private readonly List<AuthenticatorDeviceReadEntity> _authenticatorDevices;
        private readonly List<UserNotificationReadEntity> _userNotifications;

        public UserReadEntity()
        {
            this._roles = new List<RoleReadEntity>();
            this._authenticatorApps = new List<AuthenticatorAppReadEntity>();
            this._authenticatorDevices = new List<AuthenticatorDeviceReadEntity>();
            this._userNotifications = new List<UserNotificationReadEntity>();
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

        public IReadOnlyList<RoleReadEntity> Roles => this._roles.AsReadOnly();

        public IReadOnlyList<AuthenticatorAppReadEntity> AuthenticatorApps => this._authenticatorApps.AsReadOnly();

        public IReadOnlyList<AuthenticatorDeviceReadEntity> AuthenticatorDevices =>
            this._authenticatorDevices.AsReadOnly();

        public IReadOnlyList<UserNotificationReadEntity> UserNotifications => this._userNotifications.AsReadOnly();
    }
}