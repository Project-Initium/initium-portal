// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Infrastructure;

namespace Initium.Portal.Queries.Management.Entities
{
    public class TenantReadEntity : ReadEntity
    {
        public Guid Id { get; set; }

        public string Identifier { get; set; }

        public string Name { get; set; }

        public DateTime? WhenDisabled { get; set; }

        public Guid? LastLoggedInUserId { get; set; }

        public string LastLoggedInUser { get; set; }

        public DateTime? WhenLoggedIn { get; set; }

        public string SystemFeaturesJson { get; set; }
    }
}