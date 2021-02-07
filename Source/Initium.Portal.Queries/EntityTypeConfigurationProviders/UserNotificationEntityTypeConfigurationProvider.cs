// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries.EntityTypeConfigurationProviders
{
    public class UserNotificationEntityTypeConfigurationProvider : IEntityTypeConfigurationProvider
    {
        private readonly FeatureBasedTenantInfo _tenantInfo;

        public UserNotificationEntityTypeConfigurationProvider(FeatureBasedTenantInfo tenantInfo)
        {
            this._tenantInfo = tenantInfo;
        }

        public void ApplyConfigurations(ModelBuilder modelBuilder)
        {
            var userNotifications = modelBuilder.Entity<UserNotificationReadEntity>();
            userNotifications.ToTable("vwUserNotification", "Portal");
            userNotifications.HasKey(userNotification =>
                new { userNotification.NotificationId, userNotification.UserId });

            userNotifications.Property<Guid>("TenantId");
            userNotifications.HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(this._tenantInfo.Id));
        }
    }
}