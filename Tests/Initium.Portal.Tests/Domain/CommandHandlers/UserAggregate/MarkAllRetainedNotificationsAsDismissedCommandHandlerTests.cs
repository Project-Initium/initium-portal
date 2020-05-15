// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandHandlers.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using MaybeMonad;
using Moq;
using NodaTime;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.UserAggregate
{
    public class MarkAllRetainedNotificationsAsDismissedCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.UserNotifications).Returns(new List<UserNotification>
            {
                new UserNotification(TestVariables.NotificationId),
            });
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(user.Object));
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            var clock = new Mock<IClock>();

            var handler = new MarkAllRetainedNotificationsAsDismissedCommandHandler(userRepository.Object, clock.Object);
            var cmd = new MarkAllRetainedNotificationsAsDismissedCommand(TestVariables.UserId);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.UserNotifications).Returns(new List<UserNotification>
            {
                new UserNotification(TestVariables.NotificationId),
            });
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(user.Object));
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            var clock = new Mock<IClock>();

            var handler = new MarkAllRetainedNotificationsAsDismissedCommandHandler(userRepository.Object, clock.Object);
            var cmd = new MarkAllRetainedNotificationsAsDismissedCommand(TestVariables.UserId);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenUserDoesNotExist_ExpectFailedResult()
        {
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe<IUser>.Nothing);
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            var clock = new Mock<IClock>();

            var handler = new MarkAllRetainedNotificationsAsDismissedCommandHandler(userRepository.Object, clock.Object);
            var cmd = new MarkAllRetainedNotificationsAsDismissedCommand(TestVariables.UserId);

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }
    }
}