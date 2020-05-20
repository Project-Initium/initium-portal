﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.NotificationAggregate;
using Initium.Portal.Domain.CommandResults.NotificationAggregate;
using Initium.Portal.Domain.Commands.NotificationAggregate;
using Initium.Portal.Domain.Events;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.CommandHandlers.NotificationAggregate
{
    public class
        CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Result<CreateNotificationCommandResult, ErrorData>>
    {
        private readonly INotificationRepository _notificationRepository;

        public CreateNotificationCommandHandler(INotificationRepository notificationRepository)
        {
            this._notificationRepository = notificationRepository ??
                                           throw new ArgumentNullException(nameof(notificationRepository));
        }

        public async Task<Result<CreateNotificationCommandResult, ErrorData>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            var result = this.Process(request);
            var dbResult = await this._notificationRepository.UnitOfWork.SaveEntitiesAsync(cancellationToken);

            if (!dbResult)
            {
                return Result.Fail<CreateNotificationCommandResult, ErrorData>(new ErrorData(
                    ErrorCodes.SavingChanges, "Failed To Save Database"));
            }

            return result;
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