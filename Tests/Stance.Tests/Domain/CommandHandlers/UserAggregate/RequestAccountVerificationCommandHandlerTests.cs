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
using Stance.Core.Domain;
using Stance.Core.Settings;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandHandlers.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.Events;
using Xunit;

namespace Stance.Tests.Domain.CommandHandlers.UserAggregate
{
    public class RequestAccountVerificationCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(Guid.NewGuid(), "first-name", "last-name"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var clock = new Mock<IClock>();

            var handler = new RequestAccountVerificationCommandHandler(userRepository.Object, securitySettings.Object, clock.Object);
            var cmd = new RequestAccountVerificationCommand("email-address");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(Guid.NewGuid(), "first-name", "last-name"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var clock = new Mock<IClock>();

            var handler = new RequestAccountVerificationCommandHandler(userRepository.Object, securitySettings.Object, clock.Object);
            var cmd = new RequestAccountVerificationCommand("email-address");

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
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe<IUser>.Nothing);

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var clock = new Mock<IClock>();

            var handler = new RequestAccountVerificationCommandHandler(userRepository.Object, securitySettings.Object, clock.Object);
            var cmd = new RequestAccountVerificationCommand("email-address");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]

        public async Task Handle_GivenUserIsAlreadyVerified_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(Guid.NewGuid(), "first-name", "last-name"));
            user.Setup(x => x.IsVerified).Returns(true);
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var clock = new Mock<IClock>();

            var handler = new RequestAccountVerificationCommandHandler(userRepository.Object, securitySettings.Object, clock.Object);
            var cmd = new RequestAccountVerificationCommand("email-address");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserIsAlreadyVerified, result.Error.Code);
        }

        [Fact]

        public async Task Handle_GivenUserIsNotVerified_ExpectNewTokenGeneratedAndDomainEventRaised()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(Guid.NewGuid(), "first-name", "last-name"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var clock = new Mock<IClock>();

            var handler = new RequestAccountVerificationCommandHandler(userRepository.Object, securitySettings.Object, clock.Object);
            var cmd = new RequestAccountVerificationCommand("email-address");

            await handler.Handle(cmd, CancellationToken.None);

            user.Verify(x => x.GenerateNewAccountConfirmationToken(It.IsAny<DateTime>(), It.IsAny<TimeSpan>()), Times.Once);
            userRepository.Verify(x => x.Update(It.IsAny<IUser>()), Times.Once);
            user.Verify(x => x.AddDomainEvent(It.IsAny<AccountConfirmationTokenGeneratedEvent>()));
        }
    }
}