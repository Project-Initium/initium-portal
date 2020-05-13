// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Domain.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Initium.Portal.Domain.EventHandlers
{
    public class LogEmailMfaToken : INotificationHandler<EmailMfaTokenGeneratedEvent>
    {
        private readonly ILogger _logger;

        public LogEmailMfaToken(ILogger<LogEmailMfaToken> logger)
        {
            this._logger = logger;
        }

        public Task Handle(EmailMfaTokenGeneratedEvent notification, CancellationToken cancellationToken)
        {
            this._logger.LogInformation("MFA Token for email {EmailAddress} is {Token}", notification.EmailAddress,
                notification.Token);

            return Task.FromResult(0);
        }
    }
}