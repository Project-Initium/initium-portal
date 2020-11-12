// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Domain.Events.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Initium.Portal.Domain.EventHandlers.IntegrationEventHandlers
{
    public class LogPasswordResetToken : INotificationHandler<PasswordResetTokenGeneratedIntegrationEvent>
    {
        private readonly ILogger _logger;

        public LogPasswordResetToken(ILogger<LogEmailMfaToken> logger)
        {
            this._logger = logger;
        }

        public Task Handle(PasswordResetTokenGeneratedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            this._logger.LogInformation("Password Reset Token for email {EmailAddress} is {Token}", notification.EmailAddress,
                notification.Token);

            return Task.FromResult(0);
        }
    }
}