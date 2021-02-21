// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.ApiEndpoints.AuthEmail
{
    public class RequestMfaEmail : BaseAsyncEndpoint
        .WithoutRequest
        .WithResponse<BasicEndpointResponse>
    {
        private readonly IMediator _mediator;

        public RequestMfaEmail(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [ValidateAntiForgeryToken]
        [Authorize(AuthenticationSchemes = "login-partial")]
        [HttpPost("api/auth-email/request-mfa-email", Name = "RequestMfaEmailEndpoint")]
        public override async Task<ActionResult<BasicEndpointResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var result =
                await this._mediator.Send(new EmailMfaRequestedCommand(), cancellationToken);

            return this.Ok(new BasicEndpointResponse(result.IsSuccess));
        }
    }
}