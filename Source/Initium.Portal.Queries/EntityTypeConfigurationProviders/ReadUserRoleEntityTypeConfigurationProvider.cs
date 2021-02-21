// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries.EntityTypeConfigurationProviders
{
    public class ReadUserRoleEntityTypeConfigurationProvider : IEntityTypeConfigurationProvider
    {
        private readonly FeatureBasedTenantInfo _tenantInfo;

        public ReadUserRoleEntityTypeConfigurationProvider(FeatureBasedTenantInfo tenantInfo)
        {
            this._tenantInfo = tenantInfo;
        }

        public void ApplyConfigurations(ModelBuilder modelBuilder)
        {
            var userRoles = modelBuilder.Entity<UserRoleReadEntity>();

            userRoles.ToTable("vwUserRole", "Portal");
            userRoles.HasKey(userRole => new { userRole.RoleId, userRole.UserId });
            userRoles.Property<Guid>("TenantId");
            userRoles.HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(this._tenantInfo.Id));
        }
    }
}