// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandResults.UserAggregate
{
    public class InitiateAuthenticatorDeviceEnrollmentCommandResultTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var credentialCreateOptions = new CredentialCreateOptions();
            var result = new InitiateAuthenticatorDeviceEnrollmentCommandResult(credentialCreateOptions);

            Assert.Equal(credentialCreateOptions, result.Options);
        }
    }
}