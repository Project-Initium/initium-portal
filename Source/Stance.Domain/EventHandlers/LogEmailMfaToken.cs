// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Stance.Domain.Events;

namespace Stance.Domain.EventHandlers
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
            this._logger.LogInformation("MFA Token for email {EmailAddress} is {Token}", notification.UserEmailAddress,
                notification.Generated);

            return Task.FromResult(0);
        }
    }
}