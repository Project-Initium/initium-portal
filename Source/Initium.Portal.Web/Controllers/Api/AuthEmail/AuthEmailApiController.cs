﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Controllers.Api.AuthEmail
{
    public class AuthEmailApiController : Controller
    {
        private readonly IMediator _mediator;

        public AuthEmailApiController(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = "login-partial")]
        [HttpPost("api/auth-email/request-mfa-email")]
        public async Task<IActionResult> RequestMfaEmail()
        {
            var result =
                await this._mediator.Send(new EmailMfaRequestedCommand());

            return this.Json(new BasicApiResponse(result.IsSuccess));
        }
    }
}