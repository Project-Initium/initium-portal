// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries.EntityTypeConfigurationProviders
{
    public class ReadRoleEntityTypeConfigurationProvider : IEntityTypeConfigurationProvider
    {
        private readonly FeatureBasedTenantInfo _tenantInfo;

        public ReadRoleEntityTypeConfigurationProvider(FeatureBasedTenantInfo tenantInfo)
        {
            this._tenantInfo = tenantInfo;
        }

        public void ApplyConfigurations(ModelBuilder modelBuilder)
        {
            var roles = modelBuilder.Entity<RoleReadEntity>();

            roles.ToTable("vwRole", "Portal");
            roles.HasKey(role => role.Id);
            roles.Property<Guid>("TenantId");
            roles.HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(this._tenantInfo.Id));

            roles
                .HasMany(x => x.Resources)
                .WithMany(x => x.Roles)
                .UsingEntity<RoleResourceReadEntity>(
                    x => x.HasOne(xs => xs.Resource).WithMany(),
                    x => x.HasOne(xs => xs.Role).WithMany())
                .HasKey(x => new { x.ResourceId, x.RoleId });
        }
    }
}