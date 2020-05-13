// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.Controllers.Api.User.Models;
using Initium.Portal.Web.Infrastructure.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Controllers.Api.User
{
    public class UserApiController : Controller
    {
        private readonly IMediator _mediator;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        public UserApiController(IMediator mediator, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
        }

        [ResourceBasedAuthorize("user-unlock")]
        [HttpPost("api/users/unlock-account")]
        public async Task<IActionResult> UnlockAccount([FromBody] UnlockAccountRequest request)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Json(new UnlockAccountResponse(false));
            }

            var maybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (maybe.HasNoValue)
            {
                return this.Json(new UnlockAccountResponse(false));
            }

            if (maybe.Value.UserId == request.UserId)
            {
                return this.Json(new UnlockAccountResponse(false));
            }

            var result = await this._mediator.Send(new UnlockAccountCommand(request.UserId));
            return this.Json(new UnlockAccountResponse(result.IsSuccess));
        }

        [ResourceBasedAuthorize("user-disable")]
        [HttpPost("api/users/disable-account")]
        public async Task<IActionResult> DisableAccount([FromBody] DisableAccountRequest request)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Json(new DisableAccountResponse(false));
            }

            var maybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (maybe.HasNoValue)
            {
                return this.Json(new DisableAccountResponse(false));
            }

            if (maybe.Value.UserId == request.UserId)
            {
                return this.Json(new DisableAccountResponse(false));
            }

            var result = await this._mediator.Send(new DisableAccountCommand(request.UserId));
            return this.Json(new DisableAccountResponse(result.IsSuccess));
        }

        [ResourceBasedAuthorize("user-enable")]
        [HttpPost("api/users/enable-account")]
        public async Task<IActionResult> EnableAccount([FromBody] EnableAccountRequest request)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Json(new EnableAccountResponse(false));
            }

            var maybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (maybe.HasNoValue)
            {
                return this.Json(new EnableAccountResponse(false));
            }

            if (maybe.Value.UserId == request.UserId)
            {
                return this.Json(new EnableAccountResponse(false));
            }

            var result = await this._mediator.Send(new EnableAccountCommand(request.UserId));
            return this.Json(new EnableAccountResponse(result.IsSuccess));
        }
    }
}