﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Authentication;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandHandlers.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Moq;
using OtpNet;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.UserAggregate
{
    public class ValidateAppMfaCodeAgainstCurrentUserCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenNoUserAppearsToBeAuthenticate_ExpectFailedResult()
        {
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);

            var handler = new ValidateAppMfaCodeAgainstCurrentUserCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<ILogger<ValidateAppMfaCodeAgainstCurrentUserCommandHandler>>());

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
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ResultWithError.Fail(Mock.Of<IPersistenceError>()));
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateAppMfaCodeAgainstCurrentUserCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<ILogger<ValidateAppMfaCodeAgainstCurrentUserCommandHandler>>());

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
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateAppMfaCodeAgainstCurrentUserCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<ILogger<ValidateAppMfaCodeAgainstCurrentUserCommandHandler>>());

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
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe<IUser>.Nothing);
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateAppMfaCodeAgainstCurrentUserCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<ILogger<ValidateAppMfaCodeAgainstCurrentUserCommandHandler>>());

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
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateAppMfaCodeAgainstCurrentUserCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<ILogger<ValidateAppMfaCodeAgainstCurrentUserCommandHandler>>());

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
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateAppMfaCodeAgainstCurrentUserCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<ILogger<ValidateAppMfaCodeAgainstCurrentUserCommandHandler>>());

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
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<IPersistenceError>);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var handler = new ValidateAppMfaCodeAgainstCurrentUserCommandHandler(
                userRepository.Object, currentAuthenticatedUserProvider.Object, Mock.Of<ILogger<ValidateAppMfaCodeAgainstCurrentUserCommandHandler>>());

            var totp = new Totp(Base32Encoding.ToBytes("key"));

            var code = totp.ComputeTotp();

            var cmd = new ValidateAppMfaCodeAgainstCurrentUserCommand(code);
            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.NoAuthenticatorAppEnrolled, result.Error.Code);
        }
    }
}