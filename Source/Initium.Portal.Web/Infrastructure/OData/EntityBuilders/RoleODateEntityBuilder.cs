// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Queries.Entities;
using Initium.Portal.Web.Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.ModelBuilder;

namespace Initium.Portal.Web.Infrastructure.OData.EntityBuilders
{
    public class RoleODateEntityBuilder : IODataEntityBuilder
    {
        public void Configure(ODataConventionModelBuilder builder)
        {
            var role = builder.EntitySet<RoleReadEntity>("Roles");
            var function = role.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<RoleReadEntity>("Roles");

            function = role.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
        }
    }
}