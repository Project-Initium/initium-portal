// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Constants;
using Initium.Portal.Queries.Entities;
using Initium.Portal.Web.Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.ModelBuilder;

namespace Initium.Portal.Web.Infrastructure.OData.EntityBuilders
{
    public class UserNotificationODateEntityBuilder : IODataEntityBuilder
    {
        public void Configure(ODataConventionModelBuilder builder)
        {
            builder.AddEnumType(typeof(NotificationType));

            var userNotifications = builder.EntitySet<UserNotificationReadEntity>("UserNotifications");
            userNotifications.EntityType.HasKey(uN =>
                new
                {
                    uN.NotificationId, uN.UserId,
                });

            userNotifications.EntityType.Property(userNotification => userNotification.Message);
            userNotifications.EntityType.Property(userNotification => userNotification.Subject);
            userNotifications.EntityType.EnumProperty(userNotification => userNotification.Type);
            userNotifications.EntityType.Property(userNotification => userNotification.NotificationId);
            userNotifications.EntityType.Property(userNotification => userNotification.UserId);
            userNotifications.EntityType.Property(userNotification => userNotification.WhenNotified);
            userNotifications.EntityType.Property(userNotification => userNotification.WhenViewed);
            userNotifications.EntityType.Property(userNotification => userNotification.SerializedEventData);
            var function = userNotifications.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<UserNotificationReadEntity>("UserNotifications");

            function = userNotifications.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
        }
    }
}