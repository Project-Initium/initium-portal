// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Initium.Portal.Queries.Management.Contracts;
using Initium.Portal.Queries.Management.Entities;
using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.Infrastructure.ODataEndpoints;
using LinqKit;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Management.ODataEndpoints.Tenant
{
    [ODataRoutePrefix("Tenant")]
    [ResourceBasedAuthorize("tenant-list")]
    public class TenantODataController : BaseODataController<TenantReadEntity, TenantFilter>
    {
        private readonly ITenantQueryService _tenantQueryService;

        public TenantODataController(ITenantQueryService tenantQueryService)
        {
            this._tenantQueryService = tenantQueryService ?? throw new ArgumentNullException(nameof(tenantQueryService));
        }

        [ODataRoute("Tenant.Filtered")]
        public override IActionResult Filtered(ODataQueryOptions<TenantReadEntity> options, [FromBody]TenantFilter filter)
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
            ODataQueryOptions<TenantReadEntity> options,
            [FromBody]ExportableFilter<TenantFilter> filter)
        {
            if (!this.AreOptionsValid(options))
            {
                return this.BadRequest();
            }

            IQueryable query;
            IDictionary<string, string> mappings;
            if (filter == null)
            {
                query = options.ApplyTo(this._tenantQueryService.QueryableEntity);
                mappings = new Dictionary<string, string>();
            }
            else
            {
                var predicate = this.GeneratePredicate(filter.Filter);
                query = this._tenantQueryService.QueryableEntity.Where(predicate);
                mappings = filter.Mappings;
            }

            return this.File(this.GenerateCsvStream(query, options, mappings), "application/csv");
        }

        protected override ExpressionStarter<TenantReadEntity> GeneratePredicate(
            TenantFilter filter)
        {
            var predicate = PredicateBuilder.New<TenantReadEntity>(true);
            return predicate;
        }
    }
}