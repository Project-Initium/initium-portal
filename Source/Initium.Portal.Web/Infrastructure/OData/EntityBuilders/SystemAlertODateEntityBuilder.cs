// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Queries.Entities;
using Initium.Portal.Web.Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.ModelBuilder;

namespace Initium.Portal.Web.Infrastructure.OData.EntityBuilders
{
    public class SystemAlertODateEntityBuilder : IODataEntityBuilder
    {
        public void Configure(ODataConventionModelBuilder builder)
        {
            var systemAlert = builder.EntitySet<SystemAlertReadEntity>("SystemAlerts");
            var function = systemAlert.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<SystemAlertReadEntity>("SystemAlerts");

            function = systemAlert.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
        }
    }
}