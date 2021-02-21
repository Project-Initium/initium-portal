// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Common.Domain.AggregatesModel.TenantAggregate;
using Initium.Portal.Core.Database;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Infrastructure.Management.EntityTypeConfigurationProviders
{
    public class TenantEntityTypeConfigurationProvider : IEntityTypeConfigurationProvider
    {
        public void ApplyConfigurations(ModelBuilder modelBuilder)
        {
            var tenants = modelBuilder.Entity<Tenant>();
            tenants.ToTable("Tenant", "Admin");
            tenants.HasKey(tenant => tenant.Id);
            tenants.Ignore(tenant => tenant.DomainEvents);
            tenants.Ignore(tenant => tenant.IntegrationEvents);
            tenants.Property(tenant => tenant.Id).ValueGeneratedNever();
        }
    }
}