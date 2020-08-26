// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.ApiEndpoints.AuthApp
{
    public class EnrollAuthApp : BaseAsyncEndpoint<EnrollAuthApp.EndpointRequest, BasicEndpointResponse>
    {
        private readonly IMediator _mediator;

        public EnrollAuthApp(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpPost("api/auth-app/complete-enrollment", Name = "EnrollAuthAppEndpoint")]
        [ValidateAntiForgeryToken]
        public override async Task<ActionResult<BasicEndpointResponse>> HandleAsync([FromBody] EndpointRequest request, CancellationToken cancellationToken = default)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Ok(new BasicEndpointResponse(false));
            }

            var result =
                await this._mediator.Send(
                    new EnrollAuthenticatorAppCommand(request.SharedKey, request.Code), cancellationToken);
            return this.Ok(new BasicEndpointResponse(result.IsSuccess));
        }

        public class EndpointRequest
        {
            public string Code { get; set; }

            public string SharedKey { get; set; }
        }

        public class EndpointRequestValidator : AbstractValidator<EndpointRequest>
        {
            public EndpointRequestValidator()
            {
                this.RuleFor(x => x.SharedKey)
                    .NotEmpty();
                this.RuleFor(x => x.Code)
                    .NotEmpty();
            }
        }
    }
}