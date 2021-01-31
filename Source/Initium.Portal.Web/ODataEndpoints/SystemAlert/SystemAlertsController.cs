// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Entities;
using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.Infrastructure.ODataEndpoints;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Initium.Portal.Web.ODataEndpoints.SystemAlert
{
    [ResourceBasedAuthorize("system-alert-list")]
    public class SystemAlertsController : BaseODataController<SystemAlertReadEntity, SystemAlertFilter>
    {
        private readonly ISystemAlertQueryService _systemAlertQueryService;

        public SystemAlertsController(ISystemAlertQueryService systemAlertQueryService)
        {
            this._systemAlertQueryService = systemAlertQueryService;
        }

        [HttpPost]
        public override IActionResult Filtered(ODataQueryOptions<SystemAlertReadEntity> options, [FromBody]SystemAlertFilter filter)
        {
            if (!this.AreOptionsValid(options))
            {
                return this.BadRequest();
            }

            if (filter == null)
            {
                return this.Ok(options.ApplyTo(this._systemAlertQueryService.QueryableEntity));
            }

            var predicate = this.GeneratePredicate(filter);
            return this.Ok(options.ApplyTo(this._systemAlertQueryService.QueryableEntity.Where(predicate)));
        }

        [HttpPost]
        public override IActionResult FilteredExport(
            ODataQueryOptions<SystemAlertReadEntity> options,
            [FromBody]ExportableFilter<SystemAlertFilter> filter)
        {
            if (!this.AreOptionsValid(options))
            {
                return this.BadRequest();
            }

            IQueryable query;
            IDictionary<string, string> mappings;
            if (filter == null)
            {
                query = options.ApplyTo(this._systemAlertQueryService.QueryableEntity);
                mappings = new Dictionary<string, string>();
            }
            else
            {
                var predicate = this.GeneratePredicate(filter.Filter);
                query = this._systemAlertQueryService.QueryableEntity.Where(predicate);
                mappings = filter.Mappings;
            }

            return this.File(this.GenerateCsvStream(query, options, mappings), "application/csv");
        }

        protected override ExpressionStarter<SystemAlertReadEntity> GeneratePredicate(
            SystemAlertFilter filter)
        {
            var predicate = PredicateBuilder.New<SystemAlertReadEntity>(true);
            return predicate;
        }
    }
}