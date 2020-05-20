// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.NotificationAggregate;
using Initium.Portal.Domain.CommandHandlers.NotificationAggregate;
using Initium.Portal.Domain.Commands.NotificationAggregate;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.NotificationAggregate
{
    public class CreateNotificationCommandHandlerTests
    {
        [Fact]
        public async Task Handle_NotificationIsNotSaved_ExpectFailedResult()
        {
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            var notificationRepository = new Mock<INotificationRepository>();
            notificationRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            notificationRepository.Setup(x => x.Add(It.IsAny<INotification>()))
                .Returns((INotification notification) => notification);

            var commandHandler =
                new CreateNotificationCommandHandler(notificationRepository.Object);

            var command = new CreateNotificationCommand(
                "subject",
                "message",
                NotificationType.AlphaNotification,
                "serialized-event-data",
                TestVariables.Now,
                new List<Guid>
                {
                    TestVariables.UserId,
                });

            var result = await commandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_UserNotificationIsSaved_ExpectSuccessResult()
        {
            INotification notification = null;
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            var notificationRepository = new Mock<INotificationRepository>();
            notificationRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            notificationRepository.Setup(x => x.Add(It.IsAny<INotification>()))
                .Returns((INotification n) =>
                {
                    notification = n;
                    return n;
                });

            var commandHandler =
                new CreateNotificationCommandHandler(notificationRepository.Object);

            var command = new CreateNotificationCommand(
                "subject",
                "message",
                NotificationType.AlphaNotification,
                "serialized-event-data",
                TestVariables.Now,
                new List<Guid>
                {
                    TestVariables.UserId,
                });

            var result = await commandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.NotNull(notification);
            Assert.Equal(notification.Id, result.Value.NotificationId);
        }
    }
}