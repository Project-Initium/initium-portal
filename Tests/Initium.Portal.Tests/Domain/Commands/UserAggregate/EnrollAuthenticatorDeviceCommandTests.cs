// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;
using Initium.Portal.Domain.Commands.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.Commands.UserAggregate
{
    public class EnrollAuthenticatorDeviceCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var authenticatorAttestationRawResponse = new AuthenticatorAttestationRawResponse();
            var credentialCreateOptions = new CredentialCreateOptions();
            var command = new EnrollAuthenticatorDeviceCommand("name", authenticatorAttestationRawResponse, credentialCreateOptions);

            Assert.Equal("name", command.Name);
            Assert.Equal(authenticatorAttestationRawResponse, command.AuthenticatorAttestationRawResponse);
            Assert.Equal(credentialCreateOptions, command.CredentialCreateOptions);
        }
    }
}