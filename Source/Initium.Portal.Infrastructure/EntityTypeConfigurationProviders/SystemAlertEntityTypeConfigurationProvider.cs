// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.AggregatesModel.SystemAlertAggregate;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Infrastructure.EntityTypeConfigurationProviders
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
            var systemAlerts = modelBuilder.Entity<SystemAlert>();
            systemAlerts.ToTable("SystemAlert", "Messaging");
            systemAlerts.HasKey(systemAlert => systemAlert.Id);
            systemAlerts.Ignore(systemAlert => systemAlert.DomainEvents);
            systemAlerts.Ignore(systemAlert => systemAlert.IntegrationEvents);
            systemAlerts.Property(systemAlert => systemAlert.Id).ValueGeneratedNever();
            systemAlerts.Metadata.AddAnnotation("MULTI_TENANT", null);
            systemAlerts.Property<Guid>("TenantId");
            systemAlerts.HasQueryFilter(systemAlert => EF.Property<Guid>(systemAlert, "TenantId") == Guid.Parse(this._tenantInfo.Id));
        }
    }
}