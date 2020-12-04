// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Queries.Management.Entities;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Management.Infrastructure.Extensions
{
    public static class ODataConventionModelBuilderExtensions
    {
        internal static ODataConventionModelBuilder SetupTenantEntity(this ODataConventionModelBuilder builder)
        {
            var tenant = builder.EntitySet<TenantReadEntity>("Tenant");
            var function = tenant.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<TenantReadEntity>("Tenant");
            function.Namespace = "Tenant";

            function = tenant.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
            function.Namespace = "Tenant";
            return builder;
        }
    }
}