// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Core.Infrastructure;

namespace Stance.Queries.Dynamic.Entities
{
    public sealed class Role : ReadEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int ResourceCount { get; set; }

        public int UserCount { get; set; }
    }
}