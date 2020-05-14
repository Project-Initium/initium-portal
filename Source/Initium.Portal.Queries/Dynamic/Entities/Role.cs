// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Infrastructure;

namespace Initium.Portal.Queries.Dynamic.Entities
{
    public sealed class Role : ReadEntity
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int ResourceCount { get; set; }

        public int UserCount { get; set; }
    }
}