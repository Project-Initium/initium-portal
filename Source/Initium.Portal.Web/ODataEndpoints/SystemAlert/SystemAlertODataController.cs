// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.Infrastructure.ODataEndpoints;
using LinqKit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;

namespace Initium.Portal.Web.ODataEndpoints.SystemAlert
{
    [ODataRoutePrefix("SystemAlert")]
    [ResourceBasedAuthorize("system-alert-list")]
    public class SystemAlertODataController : BaseODataController<Queries.Entities.SystemAlertReadEntity, SystemAlertFilter>
    {
        private readonly ISystemAlertQueryService _systemAlertQueryService;

        public SystemAlertODataController(ISystemAlertQueryService systemAlertQueryService)
        {
            this._systemAlertQueryService = systemAlertQueryService;
        }

        [ODataRoute("SystemAlert.Filtered")]
        public override IActionResult Filtered(ODataQueryOptions<Queries.Entities.SystemAlertReadEntity> options, [FromBody]SystemAlertFilter filter)
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

        [ODataRoute("SystemAlert.FilteredExport")]
        public override IActionResult FilteredExport(
            ODataQueryOptions<Queries.Entities.SystemAlertReadEntity> options,
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

        protected override ExpressionStarter<Queries.Entities.SystemAlertReadEntity> GeneratePredicate(
            SystemAlertFilter filter)
        {
            var predicate = PredicateBuilder.New<Queries.Entities.SystemAlertReadEntity>(true);
            return predicate;
        }
    }
}