// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;

namespace Stance.Queries.TransferObjects.Role
{
    public class SimpleResourceDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public Guid ParentResourceId { get; set; }

        public List<SimpleResourceDto> Children { get; set; }
    }
}