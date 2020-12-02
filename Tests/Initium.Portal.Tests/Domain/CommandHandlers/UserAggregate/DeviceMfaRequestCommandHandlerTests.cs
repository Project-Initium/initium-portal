// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.CommandHandlers.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using MaybeMonad;
using Microsoft.Extensions.Logging;
using Moq;
using NodaTime;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandHandlers.UserAggregate
{
    public class DeviceMfaRequestCommandHandlerTests
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
                new DeviceMfaRequestCommandHandler(
                    userRepository.Object,
                    currentAuthenticatedUserProvider.Object,
                    Mock.Of<IClock>(),
                    Mock.Of<IFido2>(),
                    Mock.Of<ILogger<DeviceMfaRequestCommandHandler>>());

            var cmd = new DeviceMfaRequestCommand();

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
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
                new DeviceMfaRequestCommandHandler(
                    userRepository.Object,
                    currentAuthenticatedUserProvider.Object,
                    Mock.Of<IClock>(),
                    Mock.Of<IFido2>(),
                    Mock.Of<ILogger<DeviceMfaRequestCommandHandler>>());

            var cmd = new DeviceMfaRequestCommand();

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.UserNotFound, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenFailureToGenerateAssertionOptions_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
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
            fido2.Setup(
                    x => x.GetAssertionOptions(
                        It.IsAny<IEnumerable<PublicKeyCredentialDescriptor>>(),
                        It.IsAny<UserVerificationRequirement>(), It.IsAny<AuthenticationExtensionsClientInputs>()))
                .Throws<Fido2VerificationException>();

            var handler =
                new DeviceMfaRequestCommandHandler(
                    userRepository.Object,
                    currentAuthenticatedUserProvider.Object,
                    Mock.Of<IClock>(),
                    fido2.Object,
                    Mock.Of<ILogger<DeviceMfaRequestCommandHandler>>());

            var cmd = new DeviceMfaRequestCommand();

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsFailure);
            Assert.Equal(ErrorCodes.FidoVerificationFailed, result.Error.Code);
        }

        [Fact]
        public async Task Handle_GivenSuccessfulGenerationOfAssertionOptions_ExpectAuthAttemptLoggedAndOptionsReturned()
        {
            var user = new Mock<IUser>();
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

            var options = new AssertionOptions();
            var fido2 = new Mock<IFido2>();
            fido2.Setup(
                    x => x.GetAssertionOptions(
                        It.IsAny<IEnumerable<PublicKeyCredentialDescriptor>>(),
                        It.IsAny<UserVerificationRequirement>(), It.IsAny<AuthenticationExtensionsClientInputs>()))
                .Returns(options);

            var handler =
                new DeviceMfaRequestCommandHandler(
                    userRepository.Object,
                    currentAuthenticatedUserProvider.Object,
                    Mock.Of<IClock>(),
                    fido2.Object,
                    Mock.Of<ILogger<DeviceMfaRequestCommandHandler>>());

            var cmd = new DeviceMfaRequestCommand();

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsSuccess);
            user.Verify(x => x.ProcessPartialSuccessfulAuthenticationAttempt(It.IsAny<DateTime>(), It.IsAny<AuthenticationHistoryType>()), Times.Once);
            Assert.Equal(options, result.Value.AssertionOptions);
        }

        [Fact]
        public async Task Handle_GivenSavingFails_ExpectFailedResult()
        {
            var user = new Mock<IUser>();
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

            var options = new AssertionOptions();
            var fido2 = new Mock<IFido2>();
            fido2.Setup(
                    x => x.GetAssertionOptions(
                        It.IsAny<IEnumerable<PublicKeyCredentialDescriptor>>(),
                        It.IsAny<UserVerificationRequirement>(), It.IsAny<AuthenticationExtensionsClientInputs>()))
                .Returns(options);

            var handler =
                new DeviceMfaRequestCommandHandler(
                    userRepository.Object,
                    currentAuthenticatedUserProvider.Object,
                    Mock.Of<IClock>(),
                    fido2.Object,
                    Mock.Of<ILogger<DeviceMfaRequestCommandHandler>>());

            var cmd = new DeviceMfaRequestCommand();

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

            var options = new AssertionOptions();
            var fido2 = new Mock<IFido2>();
            fido2.Setup(
                    x => x.GetAssertionOptions(
                        It.IsAny<IEnumerable<PublicKeyCredentialDescriptor>>(),
                        It.IsAny<UserVerificationRequirement>(), It.IsAny<AuthenticationExtensionsClientInputs>()))
                .Returns(options);

            var handler =
                new DeviceMfaRequestCommandHandler(
                    userRepository.Object,
                    currentAuthenticatedUserProvider.Object,
                    Mock.Of<IClock>(),
                    fido2.Object,
                    Mock.Of<ILogger<DeviceMfaRequestCommandHandler>>());

            var cmd = new DeviceMfaRequestCommand();

            var result = await handler.Handle(cmd, CancellationToken.None);
            Assert.True(result.IsSuccess);
        }
    }
}