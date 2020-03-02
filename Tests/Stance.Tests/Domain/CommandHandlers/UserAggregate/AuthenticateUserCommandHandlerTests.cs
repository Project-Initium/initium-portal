// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Microsoft.Extensions.Options;
using Moq;
using NodaTime;
using Stance.Core.Constants;
using Stance.Core.Contracts.Domain;
using Stance.Core.Domain;
using Stance.Core.Settings;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandHandlers.UserAggregate;
using Stance.Domain.CommandResults.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.Events;
using Xunit;

namespace Stance.Tests.Domain.CommandHandlers.UserAggregate
{
    public class AuthenticateUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword(new string('*', 6)));
            user.Setup(x => x.Profile).Returns(new Profile(Guid.Empty, new string('*', 7), new string('*', 8)));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings
            {
                EnforceMfa = false,
            });

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object);
            var cmd = new AuthenticateUserCommand(new string('*', 5), new string('*', 6));
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
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

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object);
            var cmd = new AuthenticateUserCommand(new string('*', 5), new string('*', 6));
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenUserDoesExistButPasswordDoesNotVerifyAndAccountsAreNotLockable_ExpectFailedResultAndUnsuccessfulAttemptLoggedWithOutLockApplied()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword(new string('*', 5)));
            user.Setup(x => x.ProcessUnsuccessfulAuthenticationAttempt(It.IsAny<DateTime>(), It.IsAny<bool>()));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(() => new SecuritySettings { AllowedAttempts = -1 });

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object);
            var cmd = new AuthenticateUserCommand(new string('*', 5), new string('*', 6));
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.AuthenticationFailed, result.Error.Code);
            user.Verify(x => x.ProcessUnsuccessfulAuthenticationAttempt(It.IsAny<DateTime>(), false));
        }

        [Fact]
        public async Task Handle_GivenUserDoesExistButPasswordDoesNotVerifyAndAccountsAreLockableAndAttemptsAreLess_ExpectFailedResultAndUnsuccessfulAttemptLoggedWithOutLockApplied()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword(new string('*', 5)));
            user.Setup(x => x.ProcessUnsuccessfulAuthenticationAttempt(It.IsAny<DateTime>(), It.IsAny<bool>()));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(() => new SecuritySettings { AllowedAttempts = 1 });

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object);
            var cmd = new AuthenticateUserCommand(new string('*', 5), new string('*', 6));
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.AuthenticationFailed, result.Error.Code);
            user.Verify(x => x.ProcessUnsuccessfulAuthenticationAttempt(It.IsAny<DateTime>(), false));
        }

        [Fact]
        public async Task Handle_GivenUserDoesExistButPasswordDoesNotVerifyAndAccountsAreLockableAndAttemptsNotLess_ExpectFailedResultAndUnsuccessfulAttemptLoggedWithLockApplied()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword(new string('*', 5)));
            user.Setup(x => x.ProcessUnsuccessfulAuthenticationAttempt(It.IsAny<DateTime>(), It.IsAny<bool>()));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(() => new SecuritySettings { AllowedAttempts = 0 });

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object);
            var cmd = new AuthenticateUserCommand(new string('*', 5), new string('*', 6));
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.AuthenticationFailed, result.Error.Code);
            user.Verify(x => x.ProcessUnsuccessfulAuthenticationAttempt(It.IsAny<DateTime>(), true));
        }

        [Fact]
        public async Task Handle_GivenUserDoesExistAndPasswordDoesVerifyAndMfaIsNotRequired_ExpectSuccessfulResultWithCompletedStateAndSuccessfulAttemptLogged()
        {
            var user = new Mock<IUser>();
            var userId = Guid.NewGuid();
            user.Setup(x => x.Id).Returns(userId);
            user.Setup(x => x.EmailAddress).Returns(new string('*', 5));
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword(new string('*', 6)));
            user.Setup(x => x.ProcessSuccessfulAuthenticationAttempt(It.IsAny<DateTime>()));
            user.Setup(x => x.Profile).Returns(new Profile(Guid.Empty, new string('*', 7), new string('*', 8)));

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings
            {
                EnforceMfa = false,
            });

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object);
            var cmd = new AuthenticateUserCommand(new string('*', 5), new string('*', 6));
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(userId, result.Value.UserId);
            Assert.Equal(BaseAuthenticationProcessCommandResult.AuthenticationState.Completed, result.Value.AuthenticationStatus);
            user.Verify(x => x.ProcessSuccessfulAuthenticationAttempt(It.IsAny<DateTime>()));
        }

        [Fact]
        public async Task Handle_GivenUserDoesExistAndPasswordDoesVerifyAndMfaIsRequired_ExpectSuccessfulResultWithAwaitingMfaEmailCodeStateDomainEventRaisedAndAPartialAttemptLogged()
        {
            var user = new Mock<IUser>();
            var userId = Guid.NewGuid();
            user.Setup(x => x.Id).Returns(userId);
            user.Setup(x => x.EmailAddress).Returns(new string('*', 5));
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword(new string('*', 6)));

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings
            {
                EnforceMfa = true,
            });

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object);
            var cmd = new AuthenticateUserCommand(new string('*', 5), new string('*', 6));
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(userId, result.Value.UserId);
            Assert.Equal(BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaEmailCode, result.Value.AuthenticationStatus);
            user.Verify(x => x.ProcessPartialSuccessfulAuthenticationAttempt(It.IsAny<DateTime>(), AuthenticationHistoryType.EmailMfaRequested));
            user.Verify(x => x.AddDomainEvent(It.IsAny<EmailMfaTokenGeneratedEvent>()));
        }
    }
}