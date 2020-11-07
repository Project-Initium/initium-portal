// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Domain.Events.IntegrationEvents;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Initium.Portal.Web.Infrastructure.DomainEventHandlers
{
    public class LogEmailMfaToken : INotificationHandler<EmailMfaTokenGeneratedIntegrationEvent>
    {
        private readonly ILogger _logger;

        public LogEmailMfaToken(ILogger<LogEmailMfaToken> logger)
        {
            this._logger = logger;
        }

        public Task Handle(EmailMfaTokenGeneratedIntegrationEvent notification, CancellationToken cancellationToken)
        {
            this._logger.LogInformation("MFA Token for email {EmailAddress} is {Token}", notification.EmailAddress,
                notification.Token);

            return Task.FromResult(0);
        }
    }
}