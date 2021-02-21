// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.AggregatesModel.NotificationAggregate;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Infrastructure.EntityTypeConfigurationProviders
{
    public class NotificationEntityTypeConfigurationProvider : IEntityTypeConfigurationProvider
    {
        private readonly FeatureBasedTenantInfo _tenantInfo;

        public NotificationEntityTypeConfigurationProvider(FeatureBasedTenantInfo tenantInfo)
        {
            this._tenantInfo = tenantInfo;
        }

        public void ApplyConfigurations(ModelBuilder modelBuilder)
        {
            var notifications = modelBuilder.Entity<Notification>();
            notifications.ToTable("Notification", "Messaging");
            notifications.HasKey(notification => notification.Id);
            notifications.Ignore(notification => notification.DomainEvents);
            notifications.Ignore(notification => notification.IntegrationEvents);
            notifications.Property(notification => notification.Id).ValueGeneratedNever();
            notifications.Metadata.AddAnnotation("MULTI_TENANT", null);
            notifications.Property<Guid>("TenantId");
            notifications.HasQueryFilter(notification => EF.Property<Guid>(notification, "TenantId") == Guid.Parse(this._tenantInfo.Id));

            var navigation = notifications.Metadata.FindNavigation(nameof(Notification.UserNotifications));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            notifications.OwnsMany(role => role.UserNotifications, userNotifications =>
            {
                userNotifications.ToTable("vwUserNotification", "Messaging");
                userNotifications.HasKey(userNotification => userNotification.Id);
                userNotifications.Property(userNotification => userNotification.Id).ValueGeneratedNever();
                userNotifications.Ignore(userNotification => userNotification.DomainEvents);
                userNotifications.Ignore(userNotification => userNotification.IntegrationEvents);
                userNotifications.OwnedEntityType.AddAnnotation("MULTI_TENANT", null);
                userNotifications.Property<Guid>("TenantId");
            });
        }
    }
}