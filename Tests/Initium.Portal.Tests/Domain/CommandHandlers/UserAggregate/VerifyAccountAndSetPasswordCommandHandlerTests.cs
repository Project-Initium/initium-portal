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
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.UserAggregate
{
    public class VerifyAccountAndSetPasswordCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByUserBySecurityToken(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var handler = new VerifyAccountAndSetPasswordCommandHandler(userRepository.Object, Mock.Of<IClock>(), Mock.Of<ILogger<VerifyAccountAndSetPasswordCommandHandler>>());
            var cmd = new VerifyAccountAndSetPasswordCommand(
                TestVariables.SecurityTokenMappingId,
                "new-password");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var user = new Mock<IUser>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByUserBySecurityToken(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var handler = new VerifyAccountAndSetPasswordCommandHandler(userRepository.Object, Mock.Of<IClock>(), Mock.Of<ILogger<VerifyAccountAndSetPasswordCommandHandler>>());
            var cmd = new VerifyAccountAndSetPasswordCommand(
                TestVariables.SecurityTokenMappingId,
                "new-password");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenUserDoesNotExist_ExpectFailedResult()
        {
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByUserBySecurityToken(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe<IUser>.Nothing);

            var handler = new VerifyAccountAndSetPasswordCommandHandler(userRepository.Object, Mock.Of<IClock>(), Mock.Of<ILogger<VerifyAccountAndSetPasswordCommandHandler>>());
            var cmd = new VerifyAccountAndSetPasswordCommand(
                TestVariables.SecurityTokenMappingId,
                "new-password");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenUserExistsButIsVerified_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.IsVerified).Returns(true);
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByUserBySecurityToken(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var handler = new VerifyAccountAndSetPasswordCommandHandler(userRepository.Object, Mock.Of<IClock>(), Mock.Of<ILogger<VerifyAccountAndSetPasswordCommandHandler>>());
            var cmd = new VerifyAccountAndSetPasswordCommand(
                TestVariables.SecurityTokenMappingId,
                "new-password");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserIsAlreadyVerified, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenUserExists_ExpectAccountToBeVerifiedAndPasswordChangedAndTokenCompleted()
        {
            var user = new Mock<IUser>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByUserBySecurityToken(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var handler = new VerifyAccountAndSetPasswordCommandHandler(userRepository.Object, Mock.Of<IClock>(), Mock.Of<ILogger<VerifyAccountAndSetPasswordCommandHandler>>());
            var cmd = new VerifyAccountAndSetPasswordCommand(
                TestVariables.SecurityTokenMappingId,
                "new-password");

            await handler.Handle(cmd, CancellationToken.None);
            user.Verify(x => x.VerifyAccount(It.IsAny<DateTime>()), Times.Once);
            user.Verify(x => x.ChangePassword(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            user.Verify(x => x.CompleteTokenLifecycle(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Once);
            userRepository.Verify(x => x.Update(It.IsAny<IUser>()), Times.Once);
        }
    }
}