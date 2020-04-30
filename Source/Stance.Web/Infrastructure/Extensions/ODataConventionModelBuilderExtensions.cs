// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;
using Stance.Queries.Dynamic.Entities;

namespace Stance.Web.Infrastructure.Extensions
{
    public static class ODataConventionModelBuilderExtensions
    {
        internal static ODataConventionModelBuilder SetupUserEntity(this ODataConventionModelBuilder builder)
        {
            var user = builder.EntitySet<User>("User");
            var function = user.EntityType.Collection.Function("Export");
            function.Returns<FileResult>();
            function.Namespace = "User";

            function = user.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<User>("User");
            function.Namespace = "User";

            function = user.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
            function.Namespace = "User";
            return builder;
        }

        internal static ODataConventionModelBuilder SetupRoleEntity(this ODataConventionModelBuilder builder)
        {
            builder.EntitySet<Role>("Role");
            return builder;
        }
    }
}