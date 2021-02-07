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
            var users = builder.EntitySet<UserReadEntity>("User");
            users.EntityType.HasKey(user => user.Id);
            users.EntityType.Property(user => user.EmailAddress);
            users.EntityType.Property(user => user.FirstName);
            users.EntityType.Property(user => user.IsAdmin);
            users.EntityType.Property(user => user.IsLockable);
            users.EntityType.Property(user => user.IsLocked);
            users.EntityType.Property(user => user.IsVerified);
            users.EntityType.Property(user => user.LastName);
            users.EntityType.Property(user => user.WhenCreated);
            users.EntityType.Property(user => user.WhenDisabled);
            users.EntityType.Property(user => user.WhenLocked);
            users.EntityType.Property(user => user.WhenLastAuthenticated);

            var function = users.EntityType.Collection.Function("Filtered");
            function.ReturnsCollectionFromEntitySet<UserReadEntity>("Users");

            function = users.EntityType.Collection.Function("FilteredExport");
            function.Returns<FileResult>();
        }
    }
}