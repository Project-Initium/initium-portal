// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Infrastructure;

namespace Initium.Portal.Queries.Entities
{
    public class ResourceReadEntity : ReadEntity
    {
        private readonly List<RoleReadEntity> _roles;

        public ResourceReadEntity()
        {
            this._roles = new List<RoleReadEntity>();
        }

        public Guid Id { get; private set; }

        public string Name { get; private set; }

        public string NormalizedName { get; private set; }

        public Guid? ParentResourceId { get; private set; }

        public string FeatureCode { get; private set; }

        public IReadOnlyList<RoleReadEntity> Roles => this._roles.AsReadOnly();

        public ResourceReadEntity ParentResource { get; private set; }
    }
}