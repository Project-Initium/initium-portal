// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Moq;
using OtpNet;
using Stance.Core;
using Stance.Core.Constants;
using Stance.Core.Contracts;
using Stance.Core.Contracts.Domain;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandHandlers.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandHandlers.UserAggregate
{
    public class ValidateAppMfaCodeAgainstCurrentUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenNoUserAppearsToBeAuthenticate_ExpectFailedResult()
        {
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);

            var handler = new ValidateAppMfaCodeAgainstCurrentUserCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object);

            var cmd = new ValidateAppMfaCodeAgainstCurrentUserCommand("code");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>
            {
                new AuthenticatorApp(Guid.NewGuid(), "key", DateTime.UtcNow),
            });
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateAppMfaCodeAgainstCurrentUserCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object);

            var totp = new Totp(Base32Encoding.ToBytes("key"));

            var code = totp.ComputeTotp();

            var cmd = new ValidateAppMfaCodeAgainstCurrentUserCommand(code);
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>
            {
                new AuthenticatorApp(Guid.NewGuid(), "key", DateTime.UtcNow),
            });
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateAppMfaCodeAgainstCurrentUserCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object);

            var totp = new Totp(Base32Encoding.ToBytes("key"));

            var code = totp.ComputeTotp();

            var cmd = new ValidateAppMfaCodeAgainstCurrentUserCommand(code);
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
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe<IUser>.Nothing);
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateAppMfaCodeAgainstCurrentUserCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object);

            var cmd = new ValidateAppMfaCodeAgainstCurrentUserCommand("code");
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GiveUserDoesExistButCodeIsNotValid_ExpectFailedResultAndPartialAttemptLogged()
        {
            var user = new Mock<IUser>();
            var userId = Guid.NewGuid();
            user.Setup(x => x.Id).Returns(userId);
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>
            {
                new AuthenticatorApp(Guid.NewGuid(), "nonkey", DateTime.UtcNow),
            });
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateAppMfaCodeAgainstCurrentUserCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object);

            var totp = new Totp(Base32Encoding.ToBytes("key"));

            var code = totp.ComputeTotp();

            var cmd = new ValidateAppMfaCodeAgainstCurrentUserCommand(code);
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.MfaCodeNotValid, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GiveUserDoesExistAndCodeIsValid_ExpectSuccessfulResultAndSuccessfulAttemptLogged()
        {
            var user = new Mock<IUser>();
            var userId = Guid.NewGuid();
            user.Setup(x => x.Id).Returns(userId);
            user.Setup(x => x.Profile).Returns(new Profile(TestVariables.UserId, "first-name", "last-name"));
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>
            {
                new AuthenticatorApp(Guid.NewGuid(), "key", DateTime.UtcNow),
            });
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateAppMfaCodeAgainstCurrentUserCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object);

            var totp = new Totp(Base32Encoding.ToBytes("key"));

            var code = totp.ComputeTotp();

            var cmd = new ValidateAppMfaCodeAgainstCurrentUserCommand(code);
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            Assert.Equal(userId, result.Value.UserId);
        }

        [Fact]
        public async Task Handle_GiveUserDoesExistButHasNoAuthApp_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            var userId = Guid.NewGuid();
            user.Setup(x => x.Id).Returns(userId);
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>());
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateAppMfaCodeAgainstCurrentUserCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object);

            var totp = new Totp(Base32Encoding.ToBytes("key"));

            var code = totp.ComputeTotp();

            var cmd = new ValidateAppMfaCodeAgainstCurrentUserCommand(code);
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.NoAuthenticatorAppEnrolled, result.Error.Code);
        }
    }
}