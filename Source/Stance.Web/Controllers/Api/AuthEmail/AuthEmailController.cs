// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Web.Controllers.Api.AuthEmail
{
    public class AuthEmailController : Controller
    {
        private readonly IMediator _mediator;

        public AuthEmailController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [Authorize(AuthenticationSchemes = "login-partial")]
        [HttpPost("api/auth-email/request-mfa-email")]
        public async Task<IActionResult> RequestMfaEmail()
        {
            var result =
                await this._mediator.Send(new EmailMfaRequestedCommand());

            return this.Json(new
            {
                Success = result.IsSuccess,
            });
        }
    }
}