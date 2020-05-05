// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Web.Controllers.Api.User
{
    public class UserApiController : Controller
    {
        private readonly IMediator _mediator;

        public UserApiController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("api/users/unlock-account")]
        public async Task<IActionResult> UnlockAccount([FromBody] UnlockAccountRequest request)
        {
            var result = await this._mediator.Send(new UnlockAccountCommand(request.UserId));
            return this.Json(new UnlockAccountResponse(result.IsSuccess));
        }
    }
}