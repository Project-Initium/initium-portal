// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Queries.Management.Entities;
using Initium.Portal.Web.Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.ModelBuilder;

namespace Initium.Portal.Web.Management.Infrastructure.OData.EntityBuilders
{
    public class TenantODateEntityBuilder : IODataEntityBuilder
    {
        public void Configure(ODataConventionModelBuilder builder)
        {
            var tenant = builder.EntitySet<TenantReadEntity>("Tenants");
            var function = tenant.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<TenantReadEntity>("Tenants");

            function = tenant.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
        }
    }
}