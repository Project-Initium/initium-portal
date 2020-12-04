// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Infrastructure;

namespace Initium.Portal.Queries.Entities
{
    public class RoleResourceReadEntity : ReadEntity
    {
        public Guid RoleId { get; set; }

        public Guid ResourceId { get; set; }

        public RoleReadEntity Role { get; set; }

        public ResourceReadEntity Resource { get; set; }
    }
}