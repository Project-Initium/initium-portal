// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using LinqKit;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;
using Stance.Queries.Dynamic;
using Stance.Web.Infrastructure.Controllers;

namespace Stance.Web.Controllers.OData.Role
{
    [ODataRoutePrefix("Role")]
    public class RoleController : BaseODataController<Queries.Dynamic.Entities.Role, RoleFilter>
    {
        private readonly ODataContext _context;

        public RoleController(ODataContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All, MaxTop = 1000)]
        [ODataRoute("Role.Filtered")]
        public override IActionResult Filtered([FromBody]RoleFilter filter)
        {
            if (filter == null)
            {
                return this.Ok(this._context.Roles);
            }

            var predicate = this.GeneratePredicate(filter);
            return this.Ok(this._context.Roles.Where(predicate));
        }

        [ODataRoute("Role.FilteredExport")]
        public override IActionResult FilteredExport(ODataQueryOptions<Queries.Dynamic.Entities.Role> options, [FromBody]RoleFilter filter)
        {
            IQueryable query;
            if (filter == null)
            {
                query = this._context.Roles;
            }
            else
            {
                var predicate = this.GeneratePredicate(filter);
                query = this._context.Roles.Where(predicate);
            }

            return this.File(this.GenerateCsvStream(query, options), "application/csv");
        }

        protected override ExpressionStarter<Queries.Dynamic.Entities.Role> GeneratePredicate(RoleFilter filter)
        {
            var predicate = PredicateBuilder.New<Queries.Dynamic.Entities.Role>(true);
            if (filter.HasResources && !filter.HasNoResources)
            {
                predicate.And(x => x.ResourceCount > 0);
            }
            else if (filter.HasNoResources && !filter.HasResources)
            {
                predicate.And(x => x.ResourceCount > 0);
            }

            if (filter.HasUsers && !filter.HasNoUsers)
            {
                predicate.And(x => x.UserCount > 0);
            }
            else if (filter.HasNoUsers && !filter.HasUsers)
            {
                predicate.And(x => x.UserCount < 1);
            }

            return predicate;
        }
    }
}