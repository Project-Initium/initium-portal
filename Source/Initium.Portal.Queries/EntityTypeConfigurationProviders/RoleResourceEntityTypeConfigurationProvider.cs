using System;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries.EntityTypeConfigurationProviders
{
    public class RoleResourceEntityTypeConfigurationProvider : IEntityTypeConfigurationProvider
    {
        private readonly FeatureBasedTenantInfo _tenantInfo;

        public RoleResourceEntityTypeConfigurationProvider(FeatureBasedTenantInfo tenantInfo)
        {
            this._tenantInfo = tenantInfo;
        }

        public void ApplyConfigurations(ModelBuilder modelBuilder)
        {
            var roleResources = modelBuilder.Entity<RoleResourceReadEntity>();

            roleResources.ToTable("vwRoleResource", "Portal");
            roleResources.HasKey(roleResource => new { roleResource.RoleId, roleResource.ResourceId });
            roleResources.Property<Guid>("TenantId");
            roleResources.HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(this._tenantInfo.Id));

            roleResources
                .HasOne(roleResource => roleResource.Resource)
                .WithMany(resource => resource.RoleResources)
                .HasForeignKey(roleResource => roleResource.ResourceId);

            roleResources
                .HasOne(roleResource => roleResource.Role)
                .WithMany(role => role.RoleResources)
                .HasForeignKey(roleResource => roleResource.RoleId);
        }
    }
}