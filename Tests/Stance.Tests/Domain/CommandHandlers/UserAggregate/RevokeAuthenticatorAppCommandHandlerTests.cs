// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Moq;
using NodaTime;
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
    public class RevokeAuthenticatorAppCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenNoUserAppearsToBeAuthenticate_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>
            {
                new AuthenticatorApp(TestVariables.AuthenticatorAppId, "key", TestVariables.Now),
            });
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);

            var clock = new Mock<IClock>();

            var handler = new RevokeAuthenticatorAppCommandHandler(
                userRepository.Object, clock.Object, currentAuthenticatedUserProvider.Object);
            var cmd = new RevokeAuthenticatorAppCommand("password");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>
            {
                new AuthenticatorApp(TestVariables.AuthenticatorAppId, "key", TestVariables.Now),
            });
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();

            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var clock = new Mock<IClock>();

            var handler = new RevokeAuthenticatorAppCommandHandler(
                userRepository.Object, clock.Object, currentAuthenticatedUserProvider.Object);
            var cmd = new RevokeAuthenticatorAppCommand("password");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>
            {
                new AuthenticatorApp(TestVariables.AuthenticatorAppId, "key", TestVariables.Now),
            });
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var clock = new Mock<IClock>();

            var handler = new RevokeAuthenticatorAppCommandHandler(
                userRepository.Object, clock.Object, currentAuthenticatedUserProvider.Object);
            var cmd = new RevokeAuthenticatorAppCommand("password");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task Handle_GivenUserDoesExist_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>
            {
                new AuthenticatorApp(TestVariables.AuthenticatorAppId, "key", TestVariables.Now),
            });
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe<IUser>.Nothing);

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var clock = new Mock<IClock>();

            var handler = new RevokeAuthenticatorAppCommandHandler(
                userRepository.Object, clock.Object, currentAuthenticatedUserProvider.Object);
            var cmd = new RevokeAuthenticatorAppCommand("password");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenUserHasAppEnrolled_ExpectSuccessfulResultAndRevokeAttempted()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>
            {
                new AuthenticatorApp(TestVariables.AuthenticatorAppId, "key", TestVariables.Now),
            });
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var clock = new Mock<IClock>();

            var handler = new RevokeAuthenticatorAppCommandHandler(
                userRepository.Object, clock.Object, currentAuthenticatedUserProvider.Object);
            var cmd = new RevokeAuthenticatorAppCommand("password");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsSuccess);
            user.Verify(x => x.RevokeAuthenticatorApp(It.IsAny<DateTime>()), Times.Once);
        }

        [Fact]
        public async Task Handle_GivenUserHasNoAppEnrolled_ExpectFailedResultAndNoRevokeAttempted()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorApps).Returns(new List<AuthenticatorApp>());
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var clock = new Mock<IClock>();

            var handler = new RevokeAuthenticatorAppCommandHandler(
                userRepository.Object, clock.Object, currentAuthenticatedUserProvider.Object);
            var cmd = new RevokeAuthenticatorAppCommand("password");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.NoAuthenticatorAppEnrolled, result.Error.Code);
            user.Verify(x => x.RevokeAuthenticatorApp(It.IsAny<DateTime>()), Times.Never);
        }

        [Fact]
        public async Task Handle_GivenPasswordIsNotCorrect_ExpectFailedResultAndNoRevokeAttempted()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new UnauthenticatedUser(TestVariables.UserId, MfaProvider.None) as ISystemUser));

            var clock = new Mock<IClock>();

            var handler = new RevokeAuthenticatorAppCommandHandler(
                userRepository.Object, clock.Object, currentAuthenticatedUserProvider.Object);
            var cmd = new RevokeAuthenticatorAppCommand("wrong-password");

            var result = await handler.Handle(cmd, CancellationToken.None);

            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.PasswordNotCorrect, result.Error.Code);
            user.Verify(x => x.RevokeAuthenticatorApp(It.IsAny<DateTime>()), Times.Never);
        }
    }
}