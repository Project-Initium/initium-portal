﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Web.Infrastructure.ODataEndpoints;
using LinqKit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Attributes;

namespace Initium.Portal.Web.ODataEndpoints.UserNotification
{
    [ODataRoutePrefix("UserNotification")]
    [Authorize]
    public class UserNotificationController : BaseODataController<Queries.Entities.UserNotification, UserNotificationFilter>
    {
        private readonly IUserNotificationQueryService _userNotificationQueryService;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        public UserNotificationController(IUserNotificationQueryService userNotificationQueryService, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._userNotificationQueryService = userNotificationQueryService ?? throw new ArgumentNullException(nameof(userNotificationQueryService));
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ?? throw new ArgumentNullException(nameof(currentAuthenticatedUserProvider));
        }

        [ODataRoute("")]
        public IActionResult Get(ODataQueryOptions<Queries.Entities.UserNotification> options)
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return this.NotFound();
            }

            return this.Ok(options.ApplyTo(this._userNotificationQueryService.QueryableEntity));
        }

        [ODataRoute("UserNotification.Filtered")]
        public override IActionResult Filtered(ODataQueryOptions<Queries.Entities.UserNotification> options, [FromBody]UserNotificationFilter filter)
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

        [ODataRoute("UserNotification.FilteredExport")]
        public override IActionResult FilteredExport(
            ODataQueryOptions<Queries.Entities.UserNotification> options, [FromBody]ExportableFilter<UserNotificationFilter> filter)
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

        protected override ExpressionStarter<Queries.Entities.UserNotification> GeneratePredicate(UserNotificationFilter filter)
        {
            throw new System.NotImplementedException();
        }
    }
}