// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.Infrastructure.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.ApiEndpoints.User
{
    public class DisableAccount : BaseAsyncEndpoint<DisableAccount.EndpointRequest, BasicEndpointResponse>
    {
        private readonly IMediator _mediator;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        public DisableAccount(IMediator mediator, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            this._mediator = mediator;
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
        }

        [ValidateAntiForgeryToken]
        [ResourceBasedAuthorize("user-disable")]
        [HttpPost("api/users/disable-account", Name = "DisableAccountEndpoint")]
        public override async Task<ActionResult<BasicEndpointResponse>> HandleAsync([FromBody] EndpointRequest request, CancellationToken cancellationToken = default)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Ok(new BasicEndpointResponse(false));
            }

            var maybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (maybe.HasNoValue)
            {
                return this.Ok(new BasicEndpointResponse(false));
            }

            if (maybe.Value.UserId == request.UserId)
            {
                return this.Ok(new BasicEndpointResponse(false));
            }

            var result = await this._mediator.Send(new DisableAccountCommand(request.UserId));
            return this.Ok(new BasicEndpointResponse(result.IsSuccess));
        }

        public class EndpointRequest
        {
            public Guid UserId { get; set; }
        }

        public class EndpointRequestValidator : AbstractValidator<EndpointRequest>
        {
            public EndpointRequestValidator()
            {
                this.RuleFor(x => x.UserId)
                    .NotEqual(Guid.Empty);
            }
        }
    }
}