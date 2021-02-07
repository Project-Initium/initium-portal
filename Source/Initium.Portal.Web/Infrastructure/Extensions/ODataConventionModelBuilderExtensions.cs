// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Constants;
using Initium.Portal.Queries.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.ModelBuilder;

namespace Initium.Portal.Web.Infrastructure.Extensions
{
    public static class ODataConventionModelBuilderExtensions
    {
        public static ODataConventionModelBuilder SetupUserEntity(this ODataConventionModelBuilder builder)
        {
            //builder.

            builder.AddEnumType(typeof(NotificationType));
            
            var users = builder.EntitySet<UserReadEntity>("User");
            users.EntityType.HasKey(user => user.Id);
            users.EntityType.Property(user => user.EmailAddress);
            users.EntityType.Property(user => user.FirstName);
            users.EntityType.Property(user => user.IsAdmin);
            users.EntityType.Property(user => user.IsLockable);
            users.EntityType.Property(user => user.IsLocked);
            users.EntityType.Property(user => user.IsVerified);
            users.EntityType.Property(user => user.LastName);
            users.EntityType.Property(user => user.WhenCreated);
            users.EntityType.Property(user => user.WhenDisabled);
            users.EntityType.Property(user => user.WhenLocked);
            users.EntityType.Property(user => user.WhenLastAuthenticated);

            var function = users.EntityType.Collection.Function("Filtered");
            
            function.ReturnsCollectionFromEntitySet<UserReadEntity>("User");
            function.Namespace = "User";

            function = users.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
            function.Namespace = "User";
            return builder;
        }

        public static ODataConventionModelBuilder SetupRoleEntity(this ODataConventionModelBuilder builder)
        {
            var roles = builder.EntitySet<RoleReadEntity>("Role");
            roles.EntityType.HasKey(role => role.Id);
            roles.EntityType.Property(role => role.Name);
            roles.EntityType.Property(role => role.ResourceCount);
            roles.EntityType.Property(role => role.UserCount);
            
            var function = roles.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<RoleReadEntity>("Role");
            function.Namespace = "Role";

            function = roles.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
            function.Namespace = "Role";
            return builder;
        }

        public static ODataConventionModelBuilder SetupUserNotificationEntity(
            this ODataConventionModelBuilder builder)
        {
            var userNotifications = builder.EntitySet<UserNotificationReadEntity>("UserNotification");
            userNotifications.EntityType.HasKey(userNotification =>
                new
                {
                    userNotification.NotificationId, userNotification.UserId,
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
            function.ReturnsCollectionFromEntitySet<UserNotificationReadEntity>("UserNotification");
            function.Namespace = "UserNotification";

            function = userNotifications.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
            function.Namespace = "UserNotification";
            return builder;
        }

        public static ODataConventionModelBuilder SetupSystemAlertEntity(this ODataConventionModelBuilder builder)
        {
            builder.AddEnumType(typeof(SystemAlertType));
            var systemAlerts = builder.EntitySet<SystemAlertReadEntity>("SystemAlert");
            systemAlerts.EntityType.HasKey(systemAlert => systemAlert.Id);
            systemAlerts.EntityType.Property(systemAlert => systemAlert.Message);
            systemAlerts.EntityType.Property(systemAlert => systemAlert.Name);
            systemAlerts.EntityType.EnumProperty(systemAlert => systemAlert.Type);
            systemAlerts.EntityType.Property(systemAlert => systemAlert.IsActive);
            systemAlerts.EntityType.Property(systemAlert => systemAlert.WhenToHide);
            systemAlerts.EntityType.Property(systemAlert => systemAlert.WhenToShow);
            
            var function = systemAlerts.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<SystemAlertReadEntity>("SystemAlert");
            function.Namespace = "SystemAlert";

            function = systemAlerts.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
            function.Namespace = "SystemAlert";
            return builder;
        }
    }
}