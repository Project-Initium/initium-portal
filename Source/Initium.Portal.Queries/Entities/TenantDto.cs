// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Infrastructure;

namespace Initium.Portal.Queries.Entities
{
    public class TenantDto : ReadEntity
    {
        public Guid Id { get; set; }

        public string Identifier { get; set; }

        public string Name { get; set; }

        public string ConnectionString { get; set; }

        public DateTime? WhenDisabled { get; set; }
    }
}