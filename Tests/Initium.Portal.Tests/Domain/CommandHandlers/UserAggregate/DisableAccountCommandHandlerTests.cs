// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandHandlers.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.Events;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.UserAggregate
{
    public class DisableAccountCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            var clock = new Mock<IClock>();

            var logger = new Mock<ILogger<DisableAccountCommandHandler>>();

            var handler =
                new DisableAccountCommandHandler(userRepository.Object, clock.Object, logger.Object);

            var cmd = new DisableAccountCommand(TestVariables.UserId);
            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var clock = new Mock<IClock>();

            var logger = new Mock<ILogger<DisableAccountCommandHandler>>();

            var handler =
                new DisableAccountCommandHandler(userRepository.Object, clock.Object, logger.Object);

            var cmd = new DisableAccountCommand(TestVariables.UserId);
            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenUserDoesExist_ExpectFailedResult()
        {
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe<IUser>.Nothing);
            var clock = new Mock<IClock>();
            var logger = new Mock<ILogger<DisableAccountCommandHandler>>();

            var handler =
                new DisableAccountCommandHandler(userRepository.Object, clock.Object, logger.Object);

            var cmd = new DisableAccountCommand(TestVariables.UserId);
            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenUserIsDisabled_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.IsDisabled).Returns(true);
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var clock = new Mock<IClock>();
            var logger = new Mock<ILogger<DisableAccountCommandHandler>>();

            var handler =
                new DisableAccountCommandHandler(userRepository.Object, clock.Object, logger.Object);

            var cmd = new DisableAccountCommand(TestVariables.UserId);
            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserAlreadyDisabled, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenUserExists_ExpectAccountDisabledAndDomainEventRaised()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var clock = new Mock<IClock>();
            var logger = new Mock<ILogger<DisableAccountCommandHandler>>();

            var handler =
                new DisableAccountCommandHandler(userRepository.Object, clock.Object, logger.Object);

            var cmd = new DisableAccountCommand(TestVariables.UserId);
            await handler.Handle(cmd, CancellationToken.None);

            user.Verify(x => x.DisableAccount(It.IsAny<DateTime>()), Times.Once);
            user.Verify(x => x.AddDomainEvent(It.IsAny<UserDisabledEvent>()));
        }
    }
}