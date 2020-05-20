// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Queries.Dynamic;
using Initium.Portal.Web.Infrastructure.Controllers;
using LinqKit;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Routing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Controllers.OData.UserNotification
{
    [ODataRoutePrefix("UserNotification")]
    [Authorize]
    public class UserNotificationController : BaseODataController<Queries.Dynamic.Entities.UserNotification, UserNotificationFilter>
    {
        private readonly ODataContext _context;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        public UserNotificationController(ODataContext context, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ?? throw new ArgumentNullException(nameof(currentAuthenticatedUserProvider));
        }

        [EnableQuery(AllowedQueryOptions = AllowedQueryOptions.All, MaxTop = 1000)]
        [ODataRoute("UserNotification.Filtered")]
        public override IActionResult Filtered([FromBody]UserNotificationFilter filter)
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return this.NotFound();
            }

            return this.Ok(this._context.UserNotifications.Where(x => x.UserId == currentUser.Value.UserId));
        }

        [ODataRoute("UserNotification.FilteredExport")]
        public override IActionResult FilteredExport(ODataQueryOptions<Queries.Dynamic.Entities.UserNotification> options, [FromBody]UserNotificationFilter filter)
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return this.NotFound();
            }

            var query = this._context.UserNotifications.Where(x => x.UserId == currentUser.Value.UserId);
            return this.File(this.GenerateCsvStream(query, options), "application/csv");
        }

        protected override ExpressionStarter<Queries.Dynamic.Entities.UserNotification> GeneratePredicate(UserNotificationFilter filter)
        {
            throw new System.NotImplementedException();
        }
    }
}