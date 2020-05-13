// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Initium.Portal.Core.Domain;

namespace Initium.Portal.Domain.AggregatesModel.RoleAggregate
{
    public sealed class Role : Entity, IRole
    {
        private readonly List<RoleResource> _roleResources;

        public Role(Guid id, string name, IReadOnlyList<Guid> resources)
        {
            this.Id = id;
            this.Name = name;
            this._roleResources = resources.Select(x => new RoleResource(x)).ToList();
        }

        private Role()
        {
            this._roleResources = new List<RoleResource>();
        }

        public string Name { get; private set; }

        public IReadOnlyList<RoleResource> RoleResources => this._roleResources.AsReadOnly();

        public void UpdateName(string name)
        {
            this.Name = name;
        }

        public void SetResources(IReadOnlyList<Guid> resources)
        {
            var distinctResources = resources.Distinct().ToList();
            var currentResources = this._roleResources.Select(x => x.Id).ToList();
            var toAdd = distinctResources.Except(currentResources);
            var toRemove = currentResources.Except(distinctResources);

            foreach (var item in toRemove)
            {
                this._roleResources.Remove(this._roleResources.Single(x => x.Id == item));
            }

            this._roleResources.AddRange(toAdd.Select(x => new RoleResource(x)));
        }
    }
}