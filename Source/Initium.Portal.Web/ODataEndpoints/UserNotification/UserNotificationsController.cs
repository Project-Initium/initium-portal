// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Entities;
using Initium.Portal.Web.Infrastructure.ODataEndpoints;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace Initium.Portal.Web.ODataEndpoints.UserNotification
{
    [Authorize]
    public class UserNotificationsController : BaseODataController<UserNotificationReadEntity, UserNotificationFilter>
    {
        private readonly IUserNotificationQueryService _userNotificationQueryService;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        public UserNotificationsController(IUserNotificationQueryService userNotificationQueryService, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._userNotificationQueryService = userNotificationQueryService ?? throw new ArgumentNullException(nameof(userNotificationQueryService));
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ?? throw new ArgumentNullException(nameof(currentAuthenticatedUserProvider));
        }

        public IActionResult Get(ODataQueryOptions<UserNotificationReadEntity> options)
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return this.NotFound();
            }

            return this.Ok(options.ApplyTo(this._userNotificationQueryService.QueryableEntity));
        }

        public override IActionResult Filtered(ODataQueryOptions<UserNotificationReadEntity> options, [FromBody]UserNotificationFilter filter)
        {
            if (!this.AreOptionsValid(options))
            {
                return this.BadRequest();
            }

            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return this.NotFound();
            }

            return this.Ok(options.ApplyTo(this._userNotificationQueryService.QueryableEntity));
        }

        public override IActionResult FilteredExport(
            ODataQueryOptions<UserNotificationReadEntity> options, [FromBody]ExportableFilter<UserNotificationFilter> filter)
        {
            if (!this.AreOptionsValid(options))
            {
                return this.BadRequest();
            }

            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return this.NotFound();
            }

            var query = this._userNotificationQueryService.QueryableEntity;
            return this.File(this.GenerateCsvStream(query, options, new Dictionary<string, string>()), "application/csv");
        }

        protected override ExpressionStarter<UserNotificationReadEntity> GeneratePredicate(UserNotificationFilter filter)
        {
            throw new System.NotImplementedException();
        }
    }
}