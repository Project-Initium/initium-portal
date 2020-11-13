// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Infrastructure;

namespace Initium.Portal.Queries.Entities
{
    public class RoleReadEntity : ReadEntity
    {
        public RoleReadEntity()
        {
            this.RoleResources = new List<RoleResourceReadEntity>();
            this.UserRoles = new List<UserRoleReadEntity>();
        }

        public Guid Id { get;  set; }

        public string Name { get;  set; }

        public int ResourceCount { get;  set; }

        public int UserCount { get;  set; }

        public List<RoleResourceReadEntity> RoleResources { get;  set; }

        public List<UserRoleReadEntity> UserRoles { get;  set; }
    }
}