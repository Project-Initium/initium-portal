using Initium.Portal.Core.Database;
using Initium.Portal.Core.Settings;
using Initium.Portal.Queries.Management.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Initium.Portal.Queries.Management.EntityTypeConfigurationProviders
{
    public class TenantEntityTypeConfigurationProvider : IEntityTypeConfigurationProvider
    {
        private readonly MultiTenantSettings _multiTenantSettings;

        public TenantEntityTypeConfigurationProvider(IOptions<MultiTenantSettings> multiTenantSettings)
        {
            this._multiTenantSettings = multiTenantSettings.Value;
        }

        public void ApplyConfigurations(ModelBuilder modelBuilder)
        {
            var tenants = modelBuilder.Entity<TenantReadEntity>();
            tenants.ToTable("vwTenant", "Admin");
            tenants.HasKey(tenant => tenant.Id);
            tenants.HasQueryFilter(tenant => tenant.Id != this._multiTenantSettings.DefaultTenantId);
        }
    }
}