using System;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries.EntityTypeConfigurationProviders
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
            var roles = modelBuilder.Entity<RoleReadEntity>();

            roles.ToTable("vwRole", "Portal");
            roles.HasKey(role => role.Id);
            roles.Property<Guid>("TenantId");
            roles.HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(this._tenantInfo.Id));
            
            
        }
    }
}