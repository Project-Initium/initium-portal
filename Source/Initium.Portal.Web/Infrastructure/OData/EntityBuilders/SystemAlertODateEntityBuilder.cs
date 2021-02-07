// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Constants;
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
            builder.AddEnumType(typeof(SystemAlertType));
            
            var systemAlerts = builder.EntitySet<SystemAlertReadEntity>("SystemAlerts");
            
            systemAlerts.EntityType.HasKey(systemAlert => systemAlert.Id);
            systemAlerts.EntityType.Property(systemAlert => systemAlert.Message);
            systemAlerts.EntityType.Property(systemAlert => systemAlert.Name);
            systemAlerts.EntityType.EnumProperty(systemAlert => systemAlert.Type);
            systemAlerts.EntityType.Property(systemAlert => systemAlert.IsActive);
            systemAlerts.EntityType.Property(systemAlert => systemAlert.WhenToHide);
            systemAlerts.EntityType.Property(systemAlert => systemAlert.WhenToShow);
            
            var function = systemAlerts.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<SystemAlertReadEntity>("SystemAlerts");

            function = systemAlerts.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
        }
    }
}