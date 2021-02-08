// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Database;
using Initium.Portal.Core.Settings;
using Initium.Portal.Queries.Management.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Initium.Portal.Queries.Management.EntityTypeConfigurationProviders
{
    public class ReadTenantEntityTypeConfigurationProvider : IEntityTypeConfigurationProvider
    {
        private readonly MultiTenantSettings _multiTenantSettings;

        public ReadTenantEntityTypeConfigurationProvider(IOptions<MultiTenantSettings> multiTenantSettings)
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