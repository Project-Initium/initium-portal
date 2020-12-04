// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Infrastructure;

namespace Initium.Portal.Queries.Entities
{
    public class ResourceReadEntity : ReadEntity
    {
        public ResourceReadEntity()
        {
            this.RoleResources = new List<RoleResourceReadEntity>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string NormalizedName { get; set; }

        public Guid? ParentResourceId { get; set; }

        public string FeatureCode { get; set; }

        public List<RoleResourceReadEntity> RoleResources { get; set; }

        public ResourceReadEntity ParentResource { get; set; }
    }
}