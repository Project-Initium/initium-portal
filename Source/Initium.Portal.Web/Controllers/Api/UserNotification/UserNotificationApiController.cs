// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.Controllers.Api.UserNotification.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Controllers.Api.UserNotification
{
    [Authorize]
    public class UserNotificationApiController : Controller
    {
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IMediator _mediator;

        public UserNotificationApiController(
            IMediator mediator,
            ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ??
                                                     throw new ArgumentNullException(
                                                         nameof(currentAuthenticatedUserProvider));
        }

        [HttpPost("api/user-notifications/view")]
        public async Task<IActionResult> ViewedNotification([FromBody] ViewedNotificationRequest model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Json(new BasicApiResponse(false));
            }

            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return this.Json(new BasicApiResponse(false));
            }

            var result =
                await this._mediator.Send(
                    new MarkNotificationAsViewedCommand(currentUser.Value.UserId, model.NotificationId));
            return this.Json(new BasicApiResponse(result.IsSuccess));
        }

        [HttpPost("api/user-notifications/view-all")]
        public async Task<IActionResult> ViewedAllNotifications()
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return this.Json(new BasicApiResponse(false));
            }

            var result =
                await this._mediator.Send(
                    new MarkAllUnreadNotificationsAsViewedCommand(currentUser.Value.UserId));
            return this.Json(new BasicApiResponse(result.IsSuccess));
        }

        [HttpPost("api/user-notifications/dismiss")]
        public async Task<IActionResult> DismissedNotification([FromBody] DismissedNotificationRequest model)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Json(new BasicApiResponse(false));
            }

            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return this.Json(new BasicApiResponse(false));
            }

            var result =
                await this._mediator.Send(
                    new MarkNotificationAsDismissedCommand(currentUser.Value.UserId, model.NotificationId));
            return this.Json(new BasicApiResponse(result.IsSuccess));
        }

        [HttpPost("api/user-notifications/dismiss-all")]
        public async Task<IActionResult> DismissedAllNotification()
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return this.Json(new BasicApiResponse(false));
            }

            var result =
                await this._mediator.Send(
                    new MarkAllRetainedNotificationsAsDismissedCommand(currentUser.Value.UserId));
            return this.Json(new BasicApiResponse(result.IsSuccess));
        }
    }
}