// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.NotificationAggregate;
using Initium.Portal.Domain.CommandResults.NotificationAggregate;
using Initium.Portal.Domain.Commands.NotificationAggregate;
using MediatR;
using Microsoft.Extensions.Logging;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.NotificationAggregate
{
    public class CreateNotificationCommandHandler
        : IRequestHandler<CreateNotificationCommand, Result<CreateNotificationCommandResult, ErrorData>>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<CreateNotificationCommandHandler> _logger;

        public CreateNotificationCommandHandler(INotificationRepository notificationRepository, ILogger<CreateNotificationCommandHandler> logger)
        {
            this._notificationRepository = notificationRepository;
            this._logger = logger;
        }

        public async Task<Result<CreateNotificationCommandResult, ErrorData>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var result = this.Process(request);
            var dbResult = await this._notificationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (dbResult)
            {
                return result;
            }

            this._logger.LogDebug("Failed saving changes.");
            return Result.Fail<CreateNotificationCommandResult, ErrorData>(new ErrorData(
                ErrorCodes.SavingChanges, "Failed To Save Database"));
        }

        private Result<CreateNotificationCommandResult, ErrorData> Process(CreateNotificationCommand request)
        {
            var notification = this._notificationRepository.Add(new Notification(
                Guid.NewGuid(),
                request.Subject,
                request.Message,
                request.Type,
                request.SerializedEventData,
                request.WhenNotified));

            foreach (var userId in request.UserIds)
            {
                notification.SendToUser(userId);
            }

            return Result.Ok<CreateNotificationCommandResult, ErrorData>(new CreateNotificationCommandResult(notification.Id));
        }
    }
}