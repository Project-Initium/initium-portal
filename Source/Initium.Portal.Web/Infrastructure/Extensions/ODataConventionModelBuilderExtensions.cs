// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Queries.Entities;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Infrastructure.Extensions
{
    public static class ODataConventionModelBuilderExtensions
    {
        public static ODataConventionModelBuilder SetupUserEntity(this ODataConventionModelBuilder builder)
        {
            var user = builder.EntitySet<User>("User");

            var function = user.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<User>("User");
            function.Namespace = "User";

            function = user.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
            function.Namespace = "User";
            return builder;
        }

        public static ODataConventionModelBuilder SetupRoleEntity(this ODataConventionModelBuilder builder)
        {
            var role = builder.EntitySet<Role>("Role");
            var function = role.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<Role>("Role");
            function.Namespace = "Role";

            function = role.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
            function.Namespace = "Role";
            return builder;
        }

        public static ODataConventionModelBuilder SetupUserNotificationEntity(
            this ODataConventionModelBuilder builder)
        {
            var userNotification = builder.EntitySet<UserNotification>("UserNotification");
            userNotification.EntityType.HasKey(uN =>
                new
                {
                    uN.NotificationId, uN.UserId,
                });
            var function = userNotification.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<UserNotification>("UserNotification");
            function.Namespace = "UserNotification";

            function = userNotification.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
            function.Namespace = "UserNotification";
            return builder;
        }

        public static ODataConventionModelBuilder SetupSystemAlertEntity(this ODataConventionModelBuilder builder)
        {
            var systemAlert = builder.EntitySet<SystemAlert>("SystemAlert");
            var function = systemAlert.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<SystemAlert>("SystemAlert");
            function.Namespace = "SystemAlert";

            function = systemAlert.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
            function.Namespace = "SystemAlert";
            return builder;
        }
    }
}