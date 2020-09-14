// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Initium.Portal.Queries.Models.Role
{
    public class DetailedRoleModel
    {
        private readonly List<Guid> _resources;

        public DetailedRoleModel(Guid id, string name, List<Guid> resources)
        {
            this.Id = id;
            this.Name = name;
            this._resources = resources;
        }

        public Guid Id { get; }

        public string Name { get; }

        public IReadOnlyList<Guid> Resources => this._resources.AsReadOnly();
    }
}