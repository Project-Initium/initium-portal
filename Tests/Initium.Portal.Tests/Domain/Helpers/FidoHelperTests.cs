// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Fido2NetLib;
using Fido2NetLib.Objects;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.Helpers;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Domain.Helpers
{
    public class FidoHelperTests
    {
        [Fact]
        public void
            GenerateAssertionOptionsForUser_GivenGetAssertionOptionsThrowsException_ExpectFailedResult()
        {
            var fido = new Mock<IFido2>();
            fido.Setup(x => x.GetAssertionOptions(
                    It.IsAny<IEnumerable<PublicKeyCredentialDescriptor>>(),
                    It.IsAny<UserVerificationRequirement?>(), It.IsAny<AuthenticationExtensionsClientInputs>()))
                .Throws<Fido2VerificationException>();

            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorDevices).Returns(new List<AuthenticatorDevice>
            {
                new AuthenticatorDevice(
                    TestVariables.AuthenticatorDeviceId, TestVariables.Now, TestVariables.AuthenticatorDevicePublicKey,
                    TestVariables.AuthenticatorDeviceCredentialId, TestVariables.AuthenticatorDeviceAaguid, 1,
                    "name", "cred-type"),
            });

            var result = fido.Object.GenerateAssertionOptionsForUser(user.Object);
            Assert.True(result.IsFailure);
        }

        [Fact]
        public void
            GenerateAssertionOptionsForUser_GivenGetAssertionOptionsSucceeds_ExpectSuccessfulResultWithException()
        {
            var fido = new Mock<IFido2>();
            fido.Setup(x => x.GetAssertionOptions(
                    It.IsAny<IEnumerable<PublicKeyCredentialDescriptor>>(),
                    It.IsAny<UserVerificationRequirement?>(),
                    It.IsAny<AuthenticationExtensionsClientInputs>()))
                .Returns(new AssertionOptions());

            var user = new Mock<IUser>();
            user.Setup(x => x.AuthenticatorDevices).Returns(new List<AuthenticatorDevice>
            {
                new AuthenticatorDevice(
                    TestVariables.AuthenticatorDeviceId, TestVariables.Now, TestVariables.AuthenticatorDevicePublicKey,
                    TestVariables.AuthenticatorDeviceCredentialId, TestVariables.AuthenticatorDeviceAaguid, 1,
                    "name", "cred-type"),
            });

            var result = fido.Object.GenerateAssertionOptionsForUser(user.Object);
            Assert.True(result.IsSuccess);
        }
    }
}