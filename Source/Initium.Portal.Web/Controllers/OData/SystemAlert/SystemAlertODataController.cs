// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq;
using Initium.Portal.Queries;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.Infrastructure.Controllers;
using LinqKit;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Controllers.OData.SystemAlert
{
    [ODataRoutePrefix("SystemAlert")]
    [ResourceBasedAuthorize("system-alert-list")]
    public class SystemAlertODataController : BaseODataController<Queries.Entities.SystemAlert, SystemAlertFilter>
    {
        private readonly ISystemAlertQueryService _systemAlertQueryService;

        public SystemAlertODataController(ISystemAlertQueryService systemAlertQueryService)
        {
            this._systemAlertQueryService = systemAlertQueryService;
        }

        [ODataRoute("SystemAlert.Filtered")]
        public override IActionResult Filtered(ODataQueryOptions<Queries.Entities.SystemAlert> options, [FromBody]SystemAlertFilter filter)
        {
            if (filter == null)
            {
                return this.Ok(options.ApplyTo(this._systemAlertQueryService.QueryableEntity));
            }

            var predicate = this.GeneratePredicate(filter);
            return this.Ok(options.ApplyTo(this._systemAlertQueryService.QueryableEntity.Where(predicate)));
        }

        [ODataRoute("SystemAlert.FilteredExport")]
        public override IActionResult FilteredExport(
            ODataQueryOptions<Queries.Entities.SystemAlert> options,
            [FromBody]SystemAlertFilter filter)
        {
            IQueryable query;
            if (filter == null)
            {
                query = options.ApplyTo(this._systemAlertQueryService.QueryableEntity);
            }
            else
            {
                var predicate = this.GeneratePredicate(filter);
                query = options.ApplyTo(this._systemAlertQueryService.QueryableEntity.Where(predicate));
            }

            return this.File(this.GenerateCsvStream(query, options), "application/csv");
        }

        protected override ExpressionStarter<Queries.Entities.SystemAlert> GeneratePredicate(
            SystemAlertFilter filter)
        {
            var predicate = PredicateBuilder.New<Queries.Entities.SystemAlert>(true);
            return predicate;
        }
    }
}