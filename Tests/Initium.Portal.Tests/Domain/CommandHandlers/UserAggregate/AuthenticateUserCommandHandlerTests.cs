// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Settings;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandHandlers.UserAggregate;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.Events.IntegrationEvents;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using NodaTime;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.UserAggregate
{
    public class AuthenticateUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>());
            user.Setup(x => x.AuthenticatorDevices).Returns(new List<AuthenticatorDevice>());
            user.Setup(x => x.IsVerified).Returns(true);

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var fido = new Mock<IFido2>();

            var logger = new Mock<ILogger<AuthenticateUserCommandHandler>>();

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object, fido.Object, logger.Object);
            var cmd = new AuthenticateUserCommand("email-address", "password");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>());
            user.Setup(x => x.AuthenticatorDevices).Returns(new List<AuthenticatorDevice>());
            user.Setup(x => x.IsVerified).Returns(true);

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var fido = new Mock<IFido2>();

            var logger = new Mock<ILogger<AuthenticateUserCommandHandler>>();

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object, fido.Object, logger.Object);
            var cmd = new AuthenticateUserCommand("email-address", "password");
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

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            var fido = new Mock<IFido2>();

            var logger = new Mock<ILogger<AuthenticateUserCommandHandler>>();

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object, fido.Object, logger.Object);
            var cmd = new AuthenticateUserCommand("email-address", "password");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenUserIsDisabled_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>());
            user.Setup(x => x.AuthenticatorDevices).Returns(new List<AuthenticatorDevice>());
            user.Setup(x => x.IsDisabled).Returns(true);
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            var fido = new Mock<IFido2>();

            var logger = new Mock<ILogger<AuthenticateUserCommandHandler>>();

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object, fido.Object, logger.Object);
            var cmd = new AuthenticateUserCommand("email-address", "password");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.AccountIsDisabled, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenUserIsNotVerified_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>());
            user.Setup(x => x.AuthenticatorDevices).Returns(new List<AuthenticatorDevice>());
            user.Setup(x => x.IsDisabled).Returns(false);
            user.Setup(x => x.IsVerified).Returns(false);
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());
            var fido = new Mock<IFido2>();

            var logger = new Mock<ILogger<AuthenticateUserCommandHandler>>();

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object, fido.Object, logger.Object);
            var cmd = new AuthenticateUserCommand("email-address", "password");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.AccountNotVerified, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenUserDoesExistButPasswordDoesNotVerifyAndAccountsAreNotLockable_ExpectFailedResultAndUnsuccessfulAttemptLoggedWithOutLockApplied()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            user.Setup(x => x.IsVerified).Returns(true);

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(() => new SecuritySettings { AllowedAttempts = -1 });

            var fido = new Mock<IFido2>();

            var logger = new Mock<ILogger<AuthenticateUserCommandHandler>>();

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object, fido.Object, logger.Object);
            var cmd = new AuthenticateUserCommand("email-address", "wrong-password");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.AuthenticationFailed, result.Error.Code);
            user.Verify(x => x.ProcessUnsuccessfulAuthenticationAttempt(It.IsAny<DateTime>(), false));
        }

        [Fact]
        public async Task Handle_GivenUserDoesExistButPasswordDoesNotVerifyAndAccountsAreLockableAndAttemptsAreLess_ExpectFailedResultAndUnsuccessfulAttemptLoggedWithOutLockApplied()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            user.Setup(x => x.IsVerified).Returns(true);

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(() => new SecuritySettings { AllowedAttempts = 1 });

            var fido = new Mock<IFido2>();

            var logger = new Mock<ILogger<AuthenticateUserCommandHandler>>();

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object, fido.Object, logger.Object);
            var cmd = new AuthenticateUserCommand("email-address", "wrong-password");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.AuthenticationFailed, result.Error.Code);
            user.Verify(x => x.ProcessUnsuccessfulAuthenticationAttempt(It.IsAny<DateTime>(), false));
        }

        [Fact]
        public async Task Handle_GivenUserDoesExistButPasswordDoesNotVerifyAndAccountsAreLockableAndAttemptsNotLess_ExpectFailedResultAndUnsuccessfulAttemptLoggedWithLockApplied()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            user.Setup(x => x.IsVerified).Returns(true);

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(() => new SecuritySettings { AllowedAttempts = 0 });

            var fido = new Mock<IFido2>();

            var logger = new Mock<ILogger<AuthenticateUserCommandHandler>>();

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object, fido.Object, logger.Object);
            var cmd = new AuthenticateUserCommand("email-address", "wrong-password");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.AuthenticationFailed, result.Error.Code);
            user.Verify(x => x.ProcessUnsuccessfulAuthenticationAttempt(It.IsAny<DateTime>(), true));
        }

        [Fact]
        public async Task Handle_GivenUserDoesExistAndPasswordDoesVerifyButHasNoAppSetUpAndNoDeviceSetUp_ExpectSuccessfulResultWithAwaitingMfaEmailCodeStateDomainEventRaisedAndAPartialAttemptLogged()
        {
            var user = new Mock<IUser>();
            var userId = Guid.NewGuid();
            user.Setup(x => x.Id).Returns(userId);
            user.Setup(x => x.EmailAddress).Returns("email-address");
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>());
            user.Setup(x => x.AuthenticatorDevices).Returns(new List<AuthenticatorDevice>());
            user.Setup(x => x.IsVerified).Returns(true);
            user.Setup(x => x.Profile).Returns(new Profile(Guid.NewGuid(), "first-name", "last-name"));

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var fido = new Mock<IFido2>();

            var logger = new Mock<ILogger<AuthenticateUserCommandHandler>>();

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object, fido.Object, logger.Object);
            var cmd = new AuthenticateUserCommand("email-address", "password");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(userId, result.Value.UserId);
            Assert.False(result.Value.SetupMfaProviders.HasFlag(MfaProvider.App));
            Assert.True(result.Value.SetupMfaProviders.HasFlag(MfaProvider.Email));
            Assert.Equal(BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaEmailCode, result.Value.AuthenticationStatus);
            user.Verify(x => x.ProcessPartialSuccessfulAuthenticationAttempt(It.IsAny<DateTime>(), AuthenticationHistoryType.EmailMfaRequested));
            user.Verify(x => x.AddDomainEvent(It.IsAny<EmailMfaTokenGeneratedIntegrationEvent>()));
        }

        [Fact]
        public async Task Handle_GivenUserDoesExistAndPasswordDoesVerifyAndHasAppSetUp_ExpectSuccessfulResultWithAwaitingMfaAppCodeAndAPartialAttemptLogged()
        {
            var user = new Mock<IUser>();
            var userId = Guid.NewGuid();
            user.Setup(x => x.Id).Returns(userId);
            user.Setup(x => x.EmailAddress).Returns("email-address");
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>
            {
                new AuthenticatorApp(TestVariables.AuthenticatorAppId, "key", TestVariables.Now),
            });
            user.Setup(x => x.AuthenticatorDevices).Returns(new List<AuthenticatorDevice>());
            user.Setup(x => x.IsVerified).Returns(true);

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var fido = new Mock<IFido2>();

            var logger = new Mock<ILogger<AuthenticateUserCommandHandler>>();

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object, fido.Object, logger.Object);
            var cmd = new AuthenticateUserCommand("email-address", "password");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(userId, result.Value.UserId);
            Assert.True(result.Value.SetupMfaProviders.HasFlag(MfaProvider.App));
            Assert.True(result.Value.SetupMfaProviders.HasFlag(MfaProvider.Email));
            Assert.Equal(BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaAppCode, result.Value.AuthenticationStatus);
            user.Verify(x => x.ProcessPartialSuccessfulAuthenticationAttempt(It.IsAny<DateTime>(), AuthenticationHistoryType.AppMfaRequested));
        }

        [Fact]
        public async Task Handle_GivenUserDoesExistAndPasswordDoesVerifyAndHasDeviceSetUp_ExpectSuccessfulResultWithAwaitingMfaDeviceAndAPartialAttemptLogged()
        {
            var user = new Mock<IUser>();
            var userId = Guid.NewGuid();
            user.Setup(x => x.Id).Returns(userId);
            user.Setup(x => x.EmailAddress).Returns("email-address");
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>());
            user.Setup(x => x.AuthenticatorDevices).Returns(new List<AuthenticatorDevice>
            {
                new AuthenticatorDevice(
                    TestVariables.AuthenticatorDeviceId,
                    TestVariables.Now,
                    TestVariables.AuthenticatorDevicePublicKey,
                    TestVariables.AuthenticatorDeviceCredentialId,
                    TestVariables.AuthenticatorDeviceAaguid,
                    1,
                    "name",
                    "cred-type"),
            });
            user.Setup(x => x.IsVerified).Returns(true);

            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.FindByEmailAddress(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();

            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings());

            var fido = new Mock<IFido2>();
            fido.Setup(
                    x => x.GetAssertionOptions(
                    It.IsAny<IEnumerable<PublicKeyCredentialDescriptor>>(),
                    It.IsAny<UserVerificationRequirement>(), It.IsAny<AuthenticationExtensionsClientInputs>()))
                .Returns(new AssertionOptions());

            var logger = new Mock<ILogger<AuthenticateUserCommandHandler>>();

            var handler = new AuthenticateUserCommandHandler(userRepository.Object, clock.Object, securitySettings.Object, fido.Object, logger.Object);
            var cmd = new AuthenticateUserCommand("email-address", "password");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(userId, result.Value.UserId);
            Assert.True(result.Value.SetupMfaProviders.HasFlag(MfaProvider.Device));
            Assert.True(result.Value.SetupMfaProviders.HasFlag(MfaProvider.Email));
            Assert.Equal(BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaDeviceCode, result.Value.AuthenticationStatus);
            user.Verify(x => x.ProcessPartialSuccessfulAuthenticationAttempt(It.IsAny<DateTime>(), AuthenticationHistoryType.DeviceMfaRequested));
        }
    }
}