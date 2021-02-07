// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Infrastructure;

namespace Initium.Portal.Queries.Entities
{
    public class RoleReadEntity : ReadEntity
    {
        private readonly List<RoleResourceReadEntity> _roleResources;
        private readonly List<UserRoleReadEntity> _userRoles;

        public RoleReadEntity()
        {
            this._roleResources = new List<RoleResourceReadEntity>();
            this._userRoles = new List<UserRoleReadEntity>();
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public int ResourceCount { get; private set; }

        public int UserCount { get; private set; }

        public IReadOnlyList<RoleResourceReadEntity> RoleResources => this._roleResources.AsReadOnly();

        public IReadOnlyList<UserRoleReadEntity> UserRoles => this._userRoles.AsReadOnly();
    }
}