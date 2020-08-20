// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using FluentValidation;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.ApiEndpoints.UserNotification
{
    public class ViewedNotification : BaseAsyncEndpoint<ViewedNotification.EndpointRequest, BasicEndpointResponse>
    {
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IMediator _mediator;

        public ViewedNotification(ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, IMediator mediator)
        {
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ?? throw new ArgumentNullException(nameof(currentAuthenticatedUserProvider));
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [ValidateAntiForgeryToken]
        [HttpPost("api/user-notifications/view")]
        public override async Task<ActionResult<BasicEndpointResponse>> HandleAsync(EndpointRequest request, CancellationToken cancellationToken = default)
        {
            if (!this.ModelState.IsValid)
            {
                return this.Ok(new BasicEndpointResponse(false));
            }

            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return this.Ok(new BasicEndpointResponse(false));
            }

            var result =
                await this._mediator.Send(
                    new MarkNotificationAsViewedCommand(currentUser.Value.UserId, request.NotificationId));
            return this.Ok(new BasicEndpointResponse(result.IsSuccess));
        }

        public class EndpointRequest
        {
            public Guid NotificationId { get; set; }
        }

        public class EndpointRequestValidator : AbstractValidator<EndpointRequest>
        {
            public EndpointRequestValidator()
            {
                this.RuleFor(x => x.NotificationId)
                    .NotEqual(Guid.Empty);
            }
        }
    }
}