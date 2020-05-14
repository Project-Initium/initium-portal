// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Queries.Dynamic.Entities;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Infrastructure.Extensions
{
    public static class ODataConventionModelBuilderExtensions
    {
        internal static ODataConventionModelBuilder SetupUserEntity(this ODataConventionModelBuilder builder)
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

        internal static ODataConventionModelBuilder SetupRoleEntity(this ODataConventionModelBuilder builder)
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
    }
}