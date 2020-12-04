// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Runtime.CompilerServices;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Core.Settings;
using Initium.Portal.Queries.Management.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;

[assembly: InternalsVisibleTo("Initium.Portal.Tests")]

namespace Initium.Portal.Queries.Management
{
    public sealed class ManagementQueryContext : CoreQueryContext, IManagementQueryContext
    {
        private readonly MultiTenantSettings _multiTenantSettings;

        public ManagementQueryContext(FeatureBasedTenantInfo tenantInfo, IOptions<MultiTenantSettings> multiTenantSettings)
            : base(tenantInfo, multiTenantSettings)
        {
            this._multiTenantSettings = multiTenantSettings.Value;
        }

        internal ManagementQueryContext(DbContextOptions<CoreQueryContext> options, FeatureBasedTenantInfo tenantInfo, IOptions<MultiTenantSettings> multiTenantSettings)
            : base(options, tenantInfo, multiTenantSettings)
        {
            this._multiTenantSettings = multiTenantSettings.Value;
        }

        public DbSet<TenantReadEntity> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TenantReadEntity>(this.ConfigureTenant);
        }

        private void ConfigureTenant(EntityTypeBuilder<TenantReadEntity> tenants)
        {
            tenants.ToTable("vwTenant", "Admin");
            tenants.HasKey(tenant => tenant.Id);
            tenants.HasQueryFilter(tenant => tenant.Id != this._multiTenantSettings.DefaultTenantId);
        }
    }
}