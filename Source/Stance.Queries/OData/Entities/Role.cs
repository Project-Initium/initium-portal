// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Stance.Queries.OData.Entities
{
    public sealed class Role
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int ResourceCount { get; set; }
    }
}