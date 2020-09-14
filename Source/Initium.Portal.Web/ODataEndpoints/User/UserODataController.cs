// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.Infrastructure.Controllers;
using LinqKit;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.ODataEndpoints.User
{
    [ODataRoutePrefix("User")]
    [ResourceBasedAuthorize("user-list")]
    public class UserODataController : BaseODataController<Queries.Entities.User, UserFilter>
    {
        private readonly IUserQueryService _userQueryService;

        public UserODataController(IUserQueryService userQueryService)
        {
            this._userQueryService = userQueryService ?? throw new ArgumentNullException(nameof(userQueryService));
        }

        [ODataRoute("User.Filtered")]
        public override IActionResult Filtered(ODataQueryOptions<Queries.Entities.User> options, [FromBody] UserFilter filter)
        {
            if (!this.AreOptionsValid(options))
            {
                return this.BadRequest();
            }

            if (filter == null)
            {
                return this.Ok(options.ApplyTo(this._userQueryService.QueryableEntity));
            }

            var predicate = this.GeneratePredicate(filter);
            return this.Ok(options.ApplyTo(this._userQueryService.QueryableEntity.Where(predicate)));
        }

        [ODataRoute("User.FilteredExport")]
        public override IActionResult FilteredExport(ODataQueryOptions<Queries.Entities.User> options, [FromBody]UserFilter filter)
        {
            if (!this.AreOptionsValid(options))
            {
                return this.BadRequest();
            }

            IQueryable query;
            if (filter == null)
            {
                query = options.ApplyTo(this._userQueryService.QueryableEntity);
            }
            else
            {
                var predicate = this.GeneratePredicate(filter);
                query = options.ApplyTo(this._userQueryService.QueryableEntity.Where(predicate));
            }

            return this.File(this.GenerateCsvStream(query, options), "application/csv");
        }

        protected override ExpressionStarter<Queries.Entities.User> GeneratePredicate([FromBody]UserFilter filter)
        {
            var predicate = PredicateBuilder.New<Queries.Entities.User>(true);
            if (filter.Verified && !filter.Unverified)
            {
                predicate.And(x => x.IsVerified);
            }
            else if (filter.Unverified && !filter.Verified)
            {
                predicate.And(x => !x.IsVerified);
            }

            if (filter.Locked && !filter.Unlocked)
            {
                predicate.And(x => x.IsLocked);
            }
            else if (filter.Unlocked && !filter.Locked)
            {
                predicate.And(x => !x.IsLocked);
            }

            if (filter.Admin && !filter.NonAdmin)
            {
                predicate.And(x => x.IsAdmin);
            }
            else if (filter.NonAdmin && !filter.Admin)
            {
                predicate.And(x => !x.IsAdmin);
            }

            return predicate;
        }
    }
}