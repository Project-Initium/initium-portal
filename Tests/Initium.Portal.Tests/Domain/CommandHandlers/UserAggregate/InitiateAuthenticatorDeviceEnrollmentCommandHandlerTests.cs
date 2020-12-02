// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandHandlers.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.UserAggregate
{
    public class InitiateAuthenticatorDeviceEnrollmentCommandHandlerTests
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

            var handler =
                new InitiateAuthenticatorDeviceEnrollmentCommandHandler(
                    currentAuthenticatedUserProvider.Object, userRepository.Object, Mock.Of<IFido2>(), Mock.Of<ILogger<InitiateAuthenticatorDeviceEnrollmentCommandHandler>>());

            var cmd = new InitiateAuthenticatorDeviceEnrollmentCommand(AuthenticatorAttachment.CrossPlatform);

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenUser_ExpectSuccessfulResultWithNewCredentialsRequested()
        {
            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorDevices).Returns(new List<AuthenticatorDevice>
            {
                new AuthenticatorDevice(
                    TestVariables.AuthenticatorDeviceId,
                    DateTime.Now,
                    TestVariables.AuthenticatorDevicePublicKey,
                    TestVariables.AuthenticatorDeviceCredentialId,
                    TestVariables.AuthenticatorDeviceAaguid,
                    1,
                    "name",
                    "cred-type"),
            });
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

            var fido2 = new Mock<IFido2>();

            var handler =
                new InitiateAuthenticatorDeviceEnrollmentCommandHandler(
                    currentAuthenticatedUserProvider.Object, userRepository.Object, fido2.Object, Mock.Of<ILogger<InitiateAuthenticatorDeviceEnrollmentCommandHandler>>());

            var cmd = new InitiateAuthenticatorDeviceEnrollmentCommand(AuthenticatorAttachment.CrossPlatform);

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsSuccess);
            fido2.Verify(x => x.RequestNewCredential(
                It.IsAny<Fido2User>(), It.IsAny<List<PublicKeyCredentialDescriptor>>(),
                It.IsAny<AuthenticatorSelection>(), It.IsAny<AttestationConveyancePreference>(),
                It.IsAny<AuthenticationExtensionsClientInputs>()));
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

            var handler =
                new InitiateAuthenticatorDeviceEnrollmentCommandHandler(
                    currentAuthenticatedUserProvider.Object, userRepository.Object, Mock.Of<IFido2>(), Mock.Of<ILogger<InitiateAuthenticatorDeviceEnrollmentCommandHandler>>());

            var cmd = new InitiateAuthenticatorDeviceEnrollmentCommand(AuthenticatorAttachment.CrossPlatform);

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }
    }
}