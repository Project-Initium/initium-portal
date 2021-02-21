// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Database;
using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries.EntityTypeConfigurationProviders
{
    public class ReadResourceEntityTypeConfigurationProvider : IEntityTypeConfigurationProvider
    {
        public void ApplyConfigurations(ModelBuilder modelBuilder)
        {
            var resources = modelBuilder.Entity<ResourceReadEntity>();

            resources.ToTable("vwResource", "Portal");
            resources.HasKey(resource => resource.Id);
        }
    }
}