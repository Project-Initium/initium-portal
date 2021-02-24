// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Contracts.Queries;

namespace Initium.Portal.Queries.Entities
{
    public class RoleReadEntity : ReadOnlyEntity
    {
        private readonly List<ResourceReadEntity> _resources;
        private readonly List<UserReadEntity> _users;

        public RoleReadEntity()
        {
            this._resources = new List<ResourceReadEntity>();
            this._users = new List<UserReadEntity>();
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public int ResourceCount { get; private set; }

        public int UserCount { get; private set; }

        public IReadOnlyList<ResourceReadEntity> Resources => this._resources.AsReadOnly();

        public IReadOnlyList<UserReadEntity> Users => this._users.AsReadOnly();
    }
}