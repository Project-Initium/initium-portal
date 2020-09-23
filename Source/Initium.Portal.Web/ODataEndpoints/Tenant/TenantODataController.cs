// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.Infrastructure.Controllers;
using LinqKit;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.ODataEndpoints.Tenant
{
    [ODataRoutePrefix("Tenant")]
    [ResourceBasedAuthorize("tenant-list")]
    public class TenantODataController : BaseODataController<Queries.Entities.TenantDto, TenantFilter>
    {
        private readonly ITenantQueryService _tenantQueryService;

        public TenantODataController(ITenantQueryService tenantQueryService)
        {
            this._tenantQueryService = tenantQueryService;
        }

        [ODataRoute("Tenant.Filtered")]
        public override IActionResult Filtered(ODataQueryOptions<Queries.Entities.TenantDto> options, [FromBody]TenantFilter filter)
        {
            if (!this.AreOptionsValid(options))
            {
                return this.BadRequest();
            }

            if (filter == null)
            {
                return this.Ok(options.ApplyTo(this._tenantQueryService.QueryableEntity));
            }

            var predicate = this.GeneratePredicate(filter);
            return this.Ok(options.ApplyTo(this._tenantQueryService.QueryableEntity.Where(predicate)));
        }

        [ODataRoute("Tenant.FilteredExport")]
        public override IActionResult FilteredExport(
            ODataQueryOptions<Queries.Entities.TenantDto> options,
            [FromBody]TenantFilter filter)
        {
            if (!this.AreOptionsValid(options))
            {
                return this.BadRequest();
            }

            IQueryable query;
            if (filter == null)
            {
                query = options.ApplyTo(this._tenantQueryService.QueryableEntity);
            }
            else
            {
                var predicate = this.GeneratePredicate(filter);
                query = options.ApplyTo(this._tenantQueryService.QueryableEntity.Where(predicate));
            }

            return this.File(this.GenerateCsvStream(query, options), "application/csv");
        }

        protected override ExpressionStarter<Queries.Entities.TenantDto> GeneratePredicate(
            TenantFilter filter)
        {
            var predicate = PredicateBuilder.New<Queries.Entities.TenantDto>(true);
            return predicate;
        }
    }
}