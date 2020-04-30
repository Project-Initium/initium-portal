// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq;
using LinqKit;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Stance.Queries.Dynamic;
using Stance.Web.Infrastructure.Controllers;

namespace Stance.Web.Controllers.OData.User
{
    [ODataRoutePrefix("User")]
    public class UserController : BaseODataController<Queries.Dynamic.Entities.User, UserFilter>
    {
        private readonly ODataContext _context;

        public UserController(ODataContext context)
        {
            this._context = context;
        }

        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All, MaxTop = 1000)]
        [ODataRoute("")]
        public override IQueryable<Queries.Dynamic.Entities.User> Get()
        {
            return this._context.Users;
        }

        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All, MaxTop = 1000)]
        [ODataRoute("User.Filtered")]
        public override IActionResult Filtered([FromBody] UserFilter filter)
        {
             if (filter == null)
             {
                  return this.Ok(this._context.Users);
             }

             var predicate = this.GeneratePredicate(filter);
             return this.Ok(this._context.Users.Where(predicate));
        }

        [ODataRoute("User.Export")]
        public override IActionResult Export(ODataQueryOptions<Queries.Dynamic.Entities.User> options)
        {
             IQueryable query = this._context.Users;
             return this.File(this.GenerateCsvStream(query, options), "application/csv");
        }

        [ODataRoute("User.FilteredExport")]
        public override IActionResult FilteredExport(ODataQueryOptions<Queries.Dynamic.Entities.User> options, [FromBody]UserFilter filter)
        {
            IQueryable query;
            if (filter == null)
            {
                query = this._context.Users;
            }
            else
            {
                var predicate = this.GeneratePredicate(filter);
                query = this._context.Users.Where(predicate);
            }

            return this.File(this.GenerateCsvStream(query, options), "application/csv");
        }

        protected override ExpressionStarter<Queries.Dynamic.Entities.User> GeneratePredicate(UserFilter filter)
        {
            var predicate = PredicateBuilder.New<Queries.Dynamic.Entities.User>(true);
            if (filter.Verified)
            {
                predicate.And(x => x.IsVerified);
            }

            if (filter.Unverified)
            {
                predicate.And(x => !x.IsVerified);
            }

            if (filter.Locked)
            {
                predicate.And(x => x.IsLocked);
            }

            if (filter.Unlocked)
            {
                predicate.And(x => !x.IsLocked);
            }

            if (filter.Admin)
            {
                predicate.And(x => x.IsAdmin);
            }

            if (filter.NonAdmin)
            {
                predicate.And(x => !x.IsAdmin);
            }

            return predicate;
        }
    }
}