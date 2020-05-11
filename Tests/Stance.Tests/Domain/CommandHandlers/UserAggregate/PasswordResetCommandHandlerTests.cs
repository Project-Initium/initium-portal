// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Microsoft.Extensions.Options;
using Moq;
using NodaTime;
using Stance.Core.Contracts.Domain;
using Stance.Core.Domain;
using Stance.Core.Settings;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandHandlers.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.Events;
using Xunit;

namespace Stance.Tests.Domain.CommandHandlers.UserAggregate
{
    public class PasswordResetCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.PasswordHistories).Returns(new List<PasswordHistory>
            {
                new PasswordHistory(TestVariables.PasswordHistoryId, BCrypt.Net.BCrypt.HashPassword("new-password"),
                    TestVariables.Now.AddMonths(-1)),
            });

            var clock = new Mock<IClock>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x =>
                    x.FindByUserBySecurityToken(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var handler = new PasswordResetCommandHandler(userRepository.Object, clock.Object, securitySettings.Object);
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
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.PasswordHistories).Returns(new List<PasswordHistory>
            {
                new PasswordHistory(TestVariables.PasswordHistoryId, BCrypt.Net.BCrypt.HashPassword("new-password"),
                    TestVariables.Now.AddMonths(-1)),
            });

            var clock = new Mock<IClock>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x =>
                    x.FindByUserBySecurityToken(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var handler = new PasswordResetCommandHandler(userRepository.Object, clock.Object, securitySettings.Object);
            var cmd = new PasswordResetCommand(
                Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                "new-password");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenUserIsFound_ExpectPasswordChangedAndLifeCycledCompletedAndSuccessfulResultAndDomainEventRaised()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.PasswordHistories).Returns(new List<PasswordHistory>
            {
                new PasswordHistory(TestVariables.PasswordHistoryId, BCrypt.Net.BCrypt.HashPassword("new-password"),
                    TestVariables.Now.AddMonths(-1)),
            });

            var clock = new Mock<IClock>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x =>
                    x.FindByUserBySecurityToken(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var handler = new PasswordResetCommandHandler(userRepository.Object, clock.Object, securitySettings.Object);
            var cmd = new PasswordResetCommand(
                Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                "new-password");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            user.Verify(x => x.ChangePassword(It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
            user.Verify(x => x.CompleteTokenLifecycle(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Once);
            userRepository.Verify(x => x.Update(It.IsAny<IUser>()), Times.Once);
            user.Verify(x => x.AddDomainEvent(It.IsAny<PasswordChangedEvent>()), Times.Once);
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
            var securitySettings = new Mock<IOptions<SecuritySettings>>();

            var handler = new PasswordResetCommandHandler(userRepository.Object, clock.Object, securitySettings.Object);
            var cmd = new PasswordResetCommand(
                Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                "new-password");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenPasswordHasBeenUsedBefore_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.PasswordHistories).Returns(new List<PasswordHistory>
            {
                new PasswordHistory(TestVariables.PasswordHistoryId, BCrypt.Net.BCrypt.HashPassword("new-password"),
                    TestVariables.Now.AddMonths(-1)),
            });

            var clock = new Mock<IClock>();
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x =>
                    x.FindByUserBySecurityToken(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings
            {
                HistoricalLimit = 1,
            });

            var handler = new PasswordResetCommandHandler(userRepository.Object, clock.Object, securitySettings.Object);
            var cmd = new PasswordResetCommand(
                Convert.ToBase64String(Guid.NewGuid().ToByteArray()),
                "new-password");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.PasswordInHistory, result.Error.Code);
        }
    }
}