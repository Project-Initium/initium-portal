// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Initium.Portal.Queries.Models.Resource
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

        public IReadOnlyList<SimpleResourceModel> SimpleResources => this._simpleResources.AsReadOnly();

        internal Guid? ParentId { get; }

        internal void SetSimpleResources(List<SimpleResourceModel> simpleResources)
        {
            this._simpleResources.AddRange(simpleResources);
        }
    }
}