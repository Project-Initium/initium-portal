// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq;
using Initium.Portal.Queries.Dynamic;
using Initium.Portal.Web.Infrastructure.Controllers;
using LinqKit;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Controllers.OData.SystemAlert
{
    [ODataRoutePrefix("SystemAlert")]
    public class SystemAlertController : BaseODataController<Queries.Dynamic.Entities.SystemAlert, SystemAlertFilter>
    {
        private readonly ODataContext _context;

        public SystemAlertController(ODataContext context)
        {
            this._context = context;
        }

        [ODataRoute("SystemAlert.Filtered")]
        public override IActionResult Filtered(SystemAlertFilter filter)
        {
            if (filter == null)
            {
                return this.Ok(this._context.SystemAlerts);
            }

            var predicate = this.GeneratePredicate(filter);
            return this.Ok(this._context.SystemAlerts.Where(predicate));
        }

        [ODataRoute("SystemAlert.FilteredExport")]
        public override IActionResult FilteredExport(
            ODataQueryOptions<Queries.Dynamic.Entities.SystemAlert> options,
            SystemAlertFilter filter)
        {
            IQueryable query;
            if (filter == null)
            {
                query = this._context.Roles;
            }
            else
            {
                var predicate = this.GeneratePredicate(filter);
                query = this._context.SystemAlerts.Where(predicate);
            }

            return this.File(this.GenerateCsvStream(query, options), "application/csv");
        }

        protected override ExpressionStarter<Queries.Dynamic.Entities.SystemAlert> GeneratePredicate(
            SystemAlertFilter filter)
        {
            var predicate = PredicateBuilder.New<Queries.Dynamic.Entities.SystemAlert>(true);
            return predicate;
        }
    }
}