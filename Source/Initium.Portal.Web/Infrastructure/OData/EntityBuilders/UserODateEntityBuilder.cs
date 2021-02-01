// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Queries.Entities;
using Initium.Portal.Web.Infrastructure.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OData.ModelBuilder;

namespace Initium.Portal.Web.Infrastructure.OData.EntityBuilders
{
    public class UserODateEntityBuilder : IODataEntityBuilder
    {
        public void Configure(ODataConventionModelBuilder builder)
        {
            var user = builder.EntitySet<UserReadEntity>("Users");

            var function = user.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<UserReadEntity>("Users");

            function = user.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
        }
    }
}