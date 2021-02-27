// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandHandlers.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.UserAggregate
{
    public class MarkNotificationAsDismissedCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenUserNotificationDoesNotExist_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.UserNotifications).Returns(new List<UserNotification>());
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From<IUser>(user.Object));
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var commandHandler =
                new MarkNotificationAsDismissedCommandHandler(userRepository.Object, Mock.Of<IClock>(), Mock.Of<ILogger<MarkNotificationAsDismissedCommandHandler>>());

            var command = new MarkNotificationAsDismissedCommand(
                TestVariables.UserId,
                TestVariables.NotificationId);

            var result = await commandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotificationNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenUserDoesNotExist_ExpectFailedResult()
        {
            var userRepository = new Mock<IUserRepository>();
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe<IUser>.Nothing);
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var commandHandler =
                new MarkNotificationAsDismissedCommandHandler(userRepository.Object, Mock.Of<IClock>(), Mock.Of<ILogger<MarkNotificationAsDismissedCommandHandler>>());

            var command = new MarkNotificationAsDismissedCommand(
                TestVariables.UserId,
                TestVariables.NotificationId);

            var result = await commandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenUserNotificationIsNotSaved_ExpectFailedResult()
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
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ResultWithError.Fail(Mock.Of<IPersistenceError>()));
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var commandHandler =
                new MarkNotificationAsDismissedCommandHandler(userRepository.Object, Mock.Of<IClock>(), Mock.Of<ILogger<MarkNotificationAsDismissedCommandHandler>>());

            var command = new MarkNotificationAsDismissedCommand(
                TestVariables.UserId,
                TestVariables.NotificationId);

            var result = await commandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenUserNotificationIsSaved_ExpectSuccessResult()
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
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);

            var commandHandler =
                new MarkNotificationAsDismissedCommandHandler(userRepository.Object, Mock.Of<IClock>(), Mock.Of<ILogger<MarkNotificationAsDismissedCommandHandler>>());

            var command = new MarkNotificationAsDismissedCommand(
                TestVariables.UserId,
                TestVariables.NotificationId);

            var result = await commandHandler.Handle(command, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }
    }
}