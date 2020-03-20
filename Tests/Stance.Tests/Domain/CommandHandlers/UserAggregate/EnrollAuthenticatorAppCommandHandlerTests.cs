﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Moq;
using NodaTime;
using OtpNet;
using Stance.Core;
using Stance.Core.Contracts;
using Stance.Core.Contracts.Domain;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandHandlers.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandHandlers.UserAggregate
{
    public class EnrollAuthenticatorAppCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenCodeDoesNotVerify_ExpectFailedResultAndNoEnrollAttempted()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(() => new List<AuthenticatorApp>());
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new AuthenticatedUser(Guid.NewGuid(), "email-address", "first-name", "last-name") as ISystemUser));

            var commandHandler = new EnrollAuthenticatorAppCommandHandler(userRepository.Object, clock.Object,
                currentAuthenticatedUserProvider.Object);

            var key = KeyGeneration.GenerateRandomKey();
            var keyAsBase32String = Base32Encoding.ToString(key);

            var cmd = new EnrollAuthenticatorAppCommand(keyAsBase32String, new string('*', 6));
            var result = await commandHandler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.FailedVerifyingAuthenticatorCode, result.Error.Code);
            user.Verify(
                x => x.EnrollAuthenticatorApp(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_GivenCodeDoesVerify_ExpectSuccessfulResultAndEnrollAttempted()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(() => new List<AuthenticatorApp>());
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new AuthenticatedUser(Guid.NewGuid(), "email-address", "first-name", "last-name") as ISystemUser));

            var commandHandler = new EnrollAuthenticatorAppCommandHandler(userRepository.Object, clock.Object,
                currentAuthenticatedUserProvider.Object);

            var key = KeyGeneration.GenerateRandomKey();
            var keyAsBase32String = Base32Encoding.ToString(key);
            var totp = new Totp(key);
            var generated = totp.ComputeTotp();

            var cmd = new EnrollAuthenticatorAppCommand(keyAsBase32String, generated);
            var result = await commandHandler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
            user.Verify(
                x => x.EnrollAuthenticatorApp(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Once);
        }

        [Fact]
        public async Task Handle_GivenNoUserAppearsToBeAuthenticate_ExpectFailedResultAndNoEnrollAttempted()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(() => new List<AuthenticatorApp>());
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);

            var commandHandler = new EnrollAuthenticatorAppCommandHandler(userRepository.Object, clock.Object,
                currentAuthenticatedUserProvider.Object);

            var key = KeyGeneration.GenerateRandomKey();
            var keyAsBase32String = Base32Encoding.ToString(key);
            var totp = new Totp(key);
            var generated = totp.ComputeTotp();

            var cmd = new EnrollAuthenticatorAppCommand(keyAsBase32String, generated);
            var result = await commandHandler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);

            user.Verify(
                x => x.EnrollAuthenticatorApp(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(() => new List<AuthenticatorApp>());
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new AuthenticatedUser(Guid.NewGuid(), "email-address", "first-name", "last-name") as ISystemUser));

            var commandHandler = new EnrollAuthenticatorAppCommandHandler(userRepository.Object, clock.Object,
                currentAuthenticatedUserProvider.Object);

            var key = KeyGeneration.GenerateRandomKey();
            var keyAsBase32String = Base32Encoding.ToString(key);
            var totp = new Totp(key);
            var generated = totp.ComputeTotp();

            var cmd = new EnrollAuthenticatorAppCommand(keyAsBase32String, generated);
            var result = await commandHandler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(() => new List<AuthenticatorApp>());
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new AuthenticatedUser(Guid.NewGuid(), "email-address", "first-name", "last-name") as ISystemUser));

            var commandHandler = new EnrollAuthenticatorAppCommandHandler(userRepository.Object, clock.Object,
                currentAuthenticatedUserProvider.Object);

            var key = KeyGeneration.GenerateRandomKey();
            var keyAsBase32String = Base32Encoding.ToString(key);
            var totp = new Totp(key);
            var generated = totp.ComputeTotp();

            var cmd = new EnrollAuthenticatorAppCommand(keyAsBase32String, generated);
            var result = await commandHandler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenUserAlreadyHasAnAuthAppEnrolled_ExpectFailedResultAndNoEnrollAttempted()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(() => new List<AuthenticatorApp>
            {
                new AuthenticatorApp(Guid.Empty, string.Empty, DateTime.UtcNow),
            });
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var clock = new Mock<IClock>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new AuthenticatedUser(Guid.NewGuid(), "email-address", "first-name", "last-name") as ISystemUser));

            var commandHandler = new EnrollAuthenticatorAppCommandHandler(userRepository.Object, clock.Object,
                currentAuthenticatedUserProvider.Object);

            var key = KeyGeneration.GenerateRandomKey();
            var keyAsBase32String = Base32Encoding.ToString(key);
            var totp = new Totp(key);
            var generated = totp.ComputeTotp();

            var cmd = new EnrollAuthenticatorAppCommand(keyAsBase32String, generated);
            var result = await commandHandler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.AuthenticatorAppAlreadyEnrolled, result.Error.Code);

            user.Verify(
                x => x.EnrollAuthenticatorApp(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Never);
        }

        [Fact]
        public async Task Handle_GivenUserDoesNotExist_ExpectFailedResultAndNoEnrollAttempted()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(() => new List<AuthenticatorApp>());
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe<IUser>.Nothing);

            var clock = new Mock<IClock>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new AuthenticatedUser(Guid.NewGuid(), "email-address", "first-name", "last-name") as ISystemUser));

            var commandHandler = new EnrollAuthenticatorAppCommandHandler(userRepository.Object, clock.Object,
                currentAuthenticatedUserProvider.Object);

            var key = KeyGeneration.GenerateRandomKey();
            var keyAsBase32String = Base32Encoding.ToString(key);
            var totp = new Totp(key);
            var generated = totp.ComputeTotp();

            var cmd = new EnrollAuthenticatorAppCommand(keyAsBase32String, generated);
            var result = await commandHandler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);

            user.Verify(
                x => x.EnrollAuthenticatorApp(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<DateTime>()),
                Times.Never);
        }
    }
}