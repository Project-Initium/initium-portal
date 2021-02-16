// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.ApiEndpoints.AuthDevice
{
    public class RevokeDevice : BaseAsyncEndpoint<RevokeDevice.EndpointRequest, BasicEndpointResponse>
    {
        private readonly IMediator _mediator;

        public RevokeDevice(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [ValidateAntiForgeryToken]
        [HttpPost("api/auth-device/revoke-device", Name = "RevokeDeviceEndpoint")]
        [Authorize]
        public override async Task<ActionResult<BasicEndpointResponse>> HandleAsync(EndpointRequest request, CancellationToken cancellationToken = default)
        {
            var result = await this._mediator.Send(new RevokeAuthenticatorDeviceCommand(request.DeviceId, request.Password), cancellationToken);
            return this.Ok(new BasicEndpointResponse(result.IsSuccess));
        }

        public class EndpointRequest
        {
            public Guid DeviceId { get; set; }

            public string Password { get; set; }
        }

        public class EndpointRequestValidator : AbstractValidator<EndpointRequest>
        {
            public EndpointRequestValidator()
            {
                this.RuleFor(x => x.Password)
                    .NotEmpty();
                this.RuleFor(x => x.DeviceId)
                    .NotEqual(Guid.Empty);
            }
        }
    }
}