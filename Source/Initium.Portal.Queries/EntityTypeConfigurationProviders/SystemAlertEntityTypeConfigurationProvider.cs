using System;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries.EntityTypeConfigurationProviders
{
    public class SystemAlertEntityTypeConfigurationProvider : IEntityTypeConfigurationProvider
    {
        private readonly FeatureBasedTenantInfo _tenantInfo;

        public SystemAlertEntityTypeConfigurationProvider(FeatureBasedTenantInfo tenantInfo)
        {
            this._tenantInfo = tenantInfo;
        }
        
        public void ApplyConfigurations(ModelBuilder modelBuilder)
        {
            var systemAlerts = modelBuilder.Entity<SystemAlertReadEntity>();

            systemAlerts.ToTable("vwSystemAlert", "Portal");
            systemAlerts.HasKey(systemAlert => systemAlert.Id);

            systemAlerts.Property<Guid>("TenantId");
            systemAlerts.HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(this._tenantInfo.Id));
        }
    }
}