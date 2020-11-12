// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.Infrastructure.ODataEndpoints;
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
        public override IActionResult FilteredExport(
            ODataQueryOptions<Queries.Entities.User> options, [FromBody]ExportableFilter<UserFilter> filter)
        {
            if (!this.AreOptionsValid(options))
            {
                return this.BadRequest();
            }

            IQueryable query;
            IDictionary<string, string> mappings;
            if (filter == null)
            {
                query = options.ApplyTo(this._userQueryService.QueryableEntity);
                mappings = new Dictionary<string, string>();
            }
            else
            {
                var predicate = this.GeneratePredicate(filter.Filter);
                query = this._userQueryService.QueryableEntity.Where(predicate);
                mappings = filter.Mappings;
            }

            return this.File(this.GenerateCsvStream(query, options, mappings), "application/csv");
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