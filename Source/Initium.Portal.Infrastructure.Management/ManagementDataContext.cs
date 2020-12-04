// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using Initium.Portal.Common.Domain.AggregatesModel.TenantAggregate;
using Initium.Portal.Core.MultiTenant;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: InternalsVisibleTo("Initium.Portal.Tests")]

namespace Initium.Portal.Infrastructure.Admin
{
    public sealed class ManagementDataContext : CoreDataContext, IManagementDataContext
    {
        private readonly FeatureBasedTenantInfo _tenantInfo;

        public ManagementDataContext(FeatureBasedTenantInfo tenantInfo, IMediator mediator)
            : base(tenantInfo, mediator)
        {
            this._tenantInfo = tenantInfo ?? throw new ArgumentNullException(nameof(tenantInfo));
        }

        internal ManagementDataContext(DbContextOptions<CoreDataContext> options, IMediator mediator, FeatureBasedTenantInfo tenantInfo)
            : base(options, mediator, tenantInfo)
        {
            this._tenantInfo = tenantInfo ?? throw new ArgumentNullException(nameof(tenantInfo));
        }

        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tenant>(this.ConfigureTenant);
        }

        private void ConfigureTenant(EntityTypeBuilder<Tenant> tenants)
        {
            tenants.ToTable("Tenant", "Admin");
            tenants.HasKey(tenant => tenant.Id);
            tenants.Ignore(tenant => tenant.DomainEvents);
            tenants.Ignore(tenant => tenant.IntegrationEvents);
            tenants.Property(tenant => tenant.Id).ValueGeneratedNever();
        }
    }
}