// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Stance.Domain.Events;

namespace Stance.Domain.EventHandlers
{
    public class LogAccountConfirmationToken : INotificationHandler<AccountConfirmationTokenGeneratedEvent>
    {
        private readonly ILogger _logger;

        public LogAccountConfirmationToken(ILogger<LogAccountConfirmationToken> logger)
        {
            this._logger = logger;
        }

        public Task Handle(AccountConfirmationTokenGeneratedEvent notification, CancellationToken cancellationToken)
        {
            this._logger.LogInformation("Password Reset Token for email {EmailAddress} is {Token}", notification.EmailAddress,
                notification.Token);

            return Task.FromResult(0);
        }
    }
}