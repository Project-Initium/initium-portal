// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

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
            var userNotification = builder.EntitySet<UserNotificationReadEntity>("UserNotifications");
            userNotification.EntityType.HasKey(uN =>
                new
                {
                    uN.NotificationId, uN.UserId,
                });
            var function = userNotification.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<UserNotificationReadEntity>("UserNotifications");

            function = userNotification.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
        }
    }
}