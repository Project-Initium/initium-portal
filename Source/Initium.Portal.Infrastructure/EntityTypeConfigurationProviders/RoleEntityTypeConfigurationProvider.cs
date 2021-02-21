// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Infrastructure.EntityTypeConfigurationProviders
{
    public class RoleEntityTypeConfigurationProvider : IEntityTypeConfigurationProvider
    {
        private readonly FeatureBasedTenantInfo _tenantInfo;

        public RoleEntityTypeConfigurationProvider(FeatureBasedTenantInfo tenantInfo)
        {
            this._tenantInfo = tenantInfo;
        }

        public void ApplyConfigurations(ModelBuilder modelBuilder)
        {
            var roles = modelBuilder.Entity<Role>();
            roles.ToTable("Role", "AccessProtection");
            roles.HasKey(role => role.Id);
            roles.Ignore(role => role.DomainEvents);
            roles.Ignore(role => role.IntegrationEvents);
            roles.Property(role => role.Id).ValueGeneratedNever();
            roles.Metadata.AddAnnotation("MULTI_TENANT", null);
            roles.Property<Guid>("TenantId");
            roles.HasQueryFilter(role => EF.Property<Guid>(role, "TenantId") == Guid.Parse(this._tenantInfo.Id));

            roles.OwnsMany(role => role.RoleResources, roleResources =>
            {
                roleResources.ToTable("RoleResource", "AccessProtection");
                roleResources.HasKey(entity => entity.Id);
                roleResources.Property(roleResource => roleResource.Id).ValueGeneratedNever();
                roleResources.Property(roleResource => roleResource.Id).HasColumnName("ResourceId");
                roleResources.Ignore(roleResource => roleResource.DomainEvents);
                roleResources.Ignore(roleResource => roleResource.IntegrationEvents);
                roleResources.OwnedEntityType.AddAnnotation("MULTI_TENANT", null);
                roleResources.Property<Guid>("TenantId");
            }).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}