// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Stance.Queries.Models.Role
{
    public class SimpleResourceModel
    {
        private readonly List<SimpleResourceModel> _simpleResources;

        public SimpleResourceModel(Guid id, string name, Guid? parentId)
        {
            this.Id = id;
            this.Name = name;
            this.ParentId = parentId;
            this._simpleResources = new List<SimpleResourceModel>();
        }

        public Guid Id { get; }

        public string Name { get; }

        internal Guid? ParentId { get; }

        public IReadOnlyList<SimpleResourceModel> SimpleResources => this._simpleResources.AsReadOnly();

        internal void SetSimpleResources(List<SimpleResourceModel> simpleResources)
        {
            this._simpleResources.AddRange(simpleResources);
        }
    }
}