// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Initium.Portal.Web.ApiEndpoints;
using Initium.Portal.Web.Infrastructure.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Management.ApiEndpoints.Tenant
{
    public class EnableTenant : BaseAsyncEndpoint<EnableTenant.EndpointRequest, BasicEndpointResponse>
    {
        private readonly IMediator _mediator;

        public EnableTenant(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [ValidateAntiForgeryToken]
        [ResourceBasedAuthorize("tenant-enable")]
        [HttpPost("api/tenants/enable-tenant", Name = "EnableTenantEndpoint")]
        public override async Task<ActionResult<BasicEndpointResponse>> HandleAsync(EndpointRequest request, CancellationToken cancellationToken = default)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Ok(new BasicEndpointResponse(false));
            }

            var result = await this._mediator.Send(new EnableTenantCommand(request.TenantId), cancellationToken);
            return this.Ok(new BasicEndpointResponse(result.IsSuccess));
        }

        public class EndpointRequest
        {
            public Guid TenantId { get; set; }
        }

        public class EndpointRequestValidator : AbstractValidator<EndpointRequest>
        {
            public EndpointRequestValidator()
            {
                this.RuleFor(x => x.TenantId)
                    .NotEqual(Guid.Empty);
            }
        }
    }
}