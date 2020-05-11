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
using Stance.Core.Contracts;
using Stance.Core.Contracts.Domain;
using Stance.Core.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Domain.CommandHandlers.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandHandlers.UserAggregate
{
    public class RevokeAuthenticatorDeviceCommandHandlerTests
    {
        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorDevices).Returns(new List<AuthenticatorDevice>
            {
                new AuthenticatorDevice(TestVariables.AuthenticatorDeviceId, TestVariables.Now,
                    TestVariables.AuthenticatorDevicePublicKey, TestVariables.AuthenticatorDeviceCredentialId,
                    TestVariables.AuthenticatorDeviceAaguid, 1, "name", "cred-type"),
            });
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("current-password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => false);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var systemUser = new Mock<ISystemUser>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));

            var clock = new Mock<IClock>();

            var handler =
                new RevokeAuthenticatorDeviceCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);

            var cmd = new RevokeAuthenticatorDeviceCommand(TestVariables.AuthenticatorDeviceId, "password");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.SavingChanges, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSavingSucceeds_ExpectSuccessfulResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorDevices).Returns(new List<AuthenticatorDevice>
            {
                new AuthenticatorDevice(TestVariables.AuthenticatorDeviceId, TestVariables.Now,
                    TestVariables.AuthenticatorDevicePublicKey, TestVariables.AuthenticatorDeviceCredentialId,
                    TestVariables.AuthenticatorDeviceAaguid, 1, "name", "cred-type"),
            });
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var systemUser = new Mock<ISystemUser>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));

            var clock = new Mock<IClock>();

            var handler =
                new RevokeAuthenticatorDeviceCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);

            var cmd = new RevokeAuthenticatorDeviceCommand(TestVariables.AuthenticatorDeviceId, "password");

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

            var systemUser = new Mock<ISystemUser>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));

            var clock = new Mock<IClock>();

            var handler =
                new RevokeAuthenticatorDeviceCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);

            var cmd = new RevokeAuthenticatorDeviceCommand(TestVariables.AuthenticatorDeviceId, "password");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]

        public async Task Handle_GivenDeviceDoesNotExist_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorDevices).Returns(new List<AuthenticatorDevice>());
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var systemUser = new Mock<ISystemUser>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));

            var clock = new Mock<IClock>();

            var handler =
                new RevokeAuthenticatorDeviceCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);

            var cmd = new RevokeAuthenticatorDeviceCommand(TestVariables.AuthenticatorDeviceId, "password");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.DeviceNotFound, result.Error.Code);
        }

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

            var clock = new Mock<IClock>();

            var handler =
                new RevokeAuthenticatorDeviceCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);

            var cmd = new RevokeAuthenticatorDeviceCommand(TestVariables.AuthenticatorDeviceId, "password");

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenDeviceExists_ExpectDeviceToBeRevoked()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorDevices).Returns(new List<AuthenticatorDevice>
            {
                new AuthenticatorDevice(TestVariables.AuthenticatorDeviceId, TestVariables.Now,
                    TestVariables.AuthenticatorDevicePublicKey, TestVariables.AuthenticatorDeviceCredentialId,
                    TestVariables.AuthenticatorDeviceAaguid, 1, "name", "cred-type"),
            });
            user.Setup(x => x.PasswordHash).Returns(BCrypt.Net.BCrypt.HashPassword("password"));
            var userRepository = new Mock<IUserRepository>();
            var unitOfWork = new Mock<IUnitOfWork>();
            unitOfWork.Setup(x => x.SaveEntitiesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(() => true);
            userRepository.Setup(x => x.UnitOfWork).Returns(unitOfWork.Object);
            userRepository.Setup(x => x.Find(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe.From(user.Object));

            var systemUser = new Mock<ISystemUser>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));

            var clock = new Mock<IClock>();

            var handler =
                new RevokeAuthenticatorDeviceCommandHandler(userRepository.Object, currentAuthenticatedUserProvider.Object, clock.Object);

            var cmd = new RevokeAuthenticatorDeviceCommand(TestVariables.AuthenticatorDeviceId, "password");

            await handler.Handle(cmd, CancellationToken.None);

            user.Verify(x => x.RevokeAuthenticatorDevice(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Once);
            userRepository.Verify(x => x.Update(It.IsAny<IUser>()), Times.Once);
        }
    }
}