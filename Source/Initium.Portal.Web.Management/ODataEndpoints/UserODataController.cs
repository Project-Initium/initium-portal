// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.ODataEndpoints.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace Initium.Portal.Web.Management.ODataEndpoints
{
    // [Route("odata/users")]
    // //[ResourceBasedAuthorize("user-list")]
    // public class UsersODataController : ODataController
    // {
    //     private readonly IUserQueryService _userQueryService;
    //
    //     public UsersODataController(IUserQueryService userQueryService)
    //     {
    //         this._userQueryService = userQueryService ?? throw new ArgumentNullException(nameof(userQueryService));
    //     }
    //
    //     //[ODataRoute("User.Filtered")]
    //     public IActionResult Get()
    //     {
    //         return this.Ok(this._userQueryService.QueryableEntity);
    //         // if (!this.AreOptionsValid(options))
    //         // {
    //         //     return this.BadRequest();
    //         // }
    //         //
    //         // if (filter == null)
    //         // {
    //         //     return this.Ok(options.ApplyTo(this._userQueryService.QueryableEntity));
    //         // }
    //         //
    //         // var predicate = this.GeneratePredicate(filter);
    //         // return this.Ok(options.ApplyTo(this._userQueryService.QueryableEntity.Where(predicate)));
    //     }
    //
    //     // [ODataRoute("User.FilteredExport")]
    //     // public override IActionResult FilteredExport(
    //     //     ODataQueryOptions<Queries.Entities.UserReadEntity> options, [FromBody]ExportableFilter<UserFilter> filter)
    //     // {
    //     //     if (!this.AreOptionsValid(options))
    //     //     {
    //     //         return this.BadRequest();
    //     //     }
    //     //
    //     //     IQueryable query;
    //     //     IDictionary<string, string> mappings;
    //     //     if (filter == null)
    //     //     {
    //     //         query = options.ApplyTo(this._userQueryService.QueryableEntity);
    //     //         mappings = new Dictionary<string, string>();
    //     //     }
    //     //     else
    //     //     {
    //     //         var predicate = this.GeneratePredicate(filter.Filter);
    //     //         query = this._userQueryService.QueryableEntity.Where(predicate);
    //     //         mappings = filter.Mappings;
    //     //     }
    //     //
    //     //     return this.File(this.GenerateCsvStream(query, options, mappings), "application/csv");
    //     // }
    //     //
    //     // protected override ExpressionStarter<Queries.Entities.UserReadEntity> GeneratePredicate([FromBody]UserFilter filter)
    //     // {
    //     //     var predicate = PredicateBuilder.New<Queries.Entities.UserReadEntity>(true);
    //     //     if (filter.Verified && !filter.Unverified)
    //     //     {
    //     //         predicate.And(x => x.IsVerified);
    //     //     }
    //     //     else if (filter.Unverified && !filter.Verified)
    //     //     {
    //     //         predicate.And(x => !x.IsVerified);
    //     //     }
    //     //
    //     //     if (filter.Locked && !filter.Unlocked)
    //     //     {
    //     //         predicate.And(x => x.IsLocked);
    //     //     }
    //     //     else if (filter.Unlocked && !filter.Locked)
    //     //     {
    //     //         predicate.And(x => !x.IsLocked);
    //     //     }
    //     //
    //     //     if (filter.Admin && !filter.NonAdmin)
    //     //     {
    //     //         predicate.And(x => x.IsAdmin);
    //     //     }
    //     //     else if (filter.NonAdmin && !filter.Admin)
    //     //     {
    //     //         predicate.And(x => !x.IsAdmin);
    //     //     }
    //     //
    //     //     return predicate;
    //     // }
    // }
}