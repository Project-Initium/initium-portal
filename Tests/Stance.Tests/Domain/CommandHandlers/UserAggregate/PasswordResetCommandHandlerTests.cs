// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Moq;
using NodaTime;
using Stance.Core.Contracts.Domain;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandHandlers.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandHandlers.UserAggregate
{
    public class PasswordResetCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var clock = new Mock<IClock>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x =>
                    x.FindByUserBySecurityToken(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(new Mock<IUser>().Object));

            var handler = new PasswordResetCommandHandler(userRepository.Object, clock.Object);
            var cmd = new PasswordResetCommand(
                Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                new string('*', 6));
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var clock = new Mock<IClock>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x =>
                    x.FindByUserBySecurityToken(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(new Mock<IUser>().Object));

            var handler = new PasswordResetCommandHandler(userRepository.Object, clock.Object);
            var cmd = new PasswordResetCommand(
                Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                new string('*', 6));
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenUserIsFound_ExpectPasswordChangedAndLifeCycledCompletedAndSuccessfulResult()
        {
            var user = new Mock<IUser>();
            var clock = new Mock<IClock>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x =>
                    x.FindByUserBySecurityToken(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var handler = new PasswordResetCommandHandler(userRepository.Object, clock.Object);
            var cmd = new PasswordResetCommand(
                Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                new string('*', 6));
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            user.Verify(x => x.ChangePassword(It.IsAny<string>()), Times.Once);
            user.Verify(x => x.CompleteTokenLifecycle(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Once);
            userRepository.Verify(x => x.Update(It.IsAny<IUser>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenUserIsNotFound_ExpectFailedResult()
        {
            var clock = new Mock<IClock>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x =>
                    x.FindByUserBySecurityToken(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe<IUser>.Nothing);

            var handler = new PasswordResetCommandHandler(userRepository.Object, clock.Object);
            var cmd = new PasswordResetCommand(
                Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                new string('*', 6));
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }
    }
}