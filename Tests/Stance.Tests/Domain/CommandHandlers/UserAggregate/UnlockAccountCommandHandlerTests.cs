// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Microsoft.Extensions.Options;
using Moq;
using NodaTime;
using Stance.Core.Contracts.Domain;
using Stance.Core.Settings;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandHandlers.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.Events;
using Xunit;

namespace Stance.Tests.Domain.CommandHandlers.UserAggregate
{
    public class UnlockAccountCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => Maybe.From(user.Object));
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            var clock = new Mock<IClock>();
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());
            var handler =
                new UnlockAccountCommandHandler(userRepository.Object, clock.Object, securitySettings.Object);

            var cmd = new UnlockAccountCommand(TestVariables.UserId);
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
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => Maybe.From(user.Object));
            var clock = new Mock<IClock>();
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());
            var handler =
                new UnlockAccountCommandHandler(userRepository.Object, clock.Object, securitySettings.Object);

            var cmd = new UnlockAccountCommand(TestVariables.UserId);
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
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => Maybe<IUser>.Nothing);
            var clock = new Mock<IClock>();
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());
            var handler =
                new UnlockAccountCommandHandler(userRepository.Object, clock.Object, securitySettings.Object);

            var cmd = new UnlockAccountCommand(TestVariables.UserId);
            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
        }

        [Fact]
        public async Task Handle_GivenUserExists_ExpectAccountUnlockedAndPasswordResetTokenGeneratedAndDomainEventRaised()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => Maybe.From(user.Object));
            var clock = new Mock<IClock>();
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());
            var handler = new UnlockAccountCommandHandler(userRepository.Object, clock.Object, securitySettings.Object);

            var cmd = new UnlockAccountCommand(TestVariables.UserId);
            await handler.Handle(cmd, CancellationToken.None);

            user.Verify(x => x.UnlockAccount(), Times.Once);
            user.Verify(x => x.GenerateNewPasswordResetToken(It.IsAny<DateTime>(), It.IsAny<TimeSpan>()), Times.Once);
            user.Verify(x => x.AddDomainEvent(It.IsAny<PasswordResetTokenGeneratedEvent>()));
        }
    }
}