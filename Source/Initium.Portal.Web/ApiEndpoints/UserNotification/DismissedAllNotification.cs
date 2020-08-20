// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Domain.Commands.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.ApiEndpoints.UserNotification
{
    public class DismissedAllNotification : BaseAsyncEndpoint<BasicEndpointResponse>
    {
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IMediator _mediator;

        public DismissedAllNotification(ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, IMediator mediator)
        {
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ?? throw new ArgumentNullException(nameof(currentAuthenticatedUserProvider));
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [ValidateAntiForgeryToken]
        [HttpPost("api/user-notifications/dismiss-all")]
        public override async Task<ActionResult<BasicEndpointResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return this.Ok(new BasicEndpointResponse(false));
            }

            var result =
                await this._mediator.Send(
                    new MarkAllRetainedNotificationsAsDismissedCommand(currentUser.Value.UserId), cancellationToken);
            return this.Ok(new BasicEndpointResponse(result.IsSuccess));
        }
    }
}