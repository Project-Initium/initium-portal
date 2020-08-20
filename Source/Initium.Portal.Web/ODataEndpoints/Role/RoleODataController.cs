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

namespace Initium.Portal.Web.ODataEndpoints.Role
{
    [ResourceBasedAuthorize("role-list")]
    [ODataRoutePrefix("Role")]
    public class RoleODataController : BaseODataController<Queries.Entities.Role, RoleFilter>
    {
        private readonly IRoleQueryService _roleQueryService;

        public RoleODataController(IRoleQueryService roleQueryService)
        {
            this._roleQueryService = roleQueryService ?? throw new ArgumentNullException(nameof(roleQueryService));
        }

        [ODataRoute("Role.Filtered")]
        public override IActionResult Filtered(ODataQueryOptions<Queries.Entities.Role> options, [FromBody]RoleFilter filter)
        {
            if (!this.AreOptionsValid(options))
            {
                return this.BadRequest();
            }

            if (filter == null)
            {
                return this.Ok(options.ApplyTo(this._roleQueryService.QueryableEntity));
            }

            var predicate = this.GeneratePredicate(filter);
            return this.Ok(options.ApplyTo(this._roleQueryService.QueryableEntity.Where(predicate)));
        }

        [ODataRoute("Role.FilteredExport")]
        public override IActionResult FilteredExport(ODataQueryOptions<Queries.Entities.Role> options, [FromBody]RoleFilter filter)
        {
            if (!this.AreOptionsValid(options))
            {
                return this.BadRequest();
            }

            IQueryable query;
            if (filter == null)
            {
                query = options.ApplyTo(this._roleQueryService.QueryableEntity);
            }
            else
            {
                var predicate = this.GeneratePredicate(filter);
                query = options.ApplyTo(this._roleQueryService.QueryableEntity.Where(predicate));
            }

            return this.File(this.GenerateCsvStream(query, options), "application/csv");
        }

        protected override ExpressionStarter<Queries.Entities.Role> GeneratePredicate(RoleFilter filter)
        {
            var predicate = PredicateBuilder.New<Queries.Entities.Role>(true);
            if (filter.HasResources && !filter.HasNoResources)
            {
                predicate.And(x => x.ResourceCount > 0);
            }
            else if (filter.HasNoResources && !filter.HasResources)
            {
                predicate.And(x => x.ResourceCount < 1);
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