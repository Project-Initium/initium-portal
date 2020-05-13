// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandResults.UserAggregate
{
    public class EnrollAuthenticatorDeviceCommandResultTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var credentialMakeResult = new Fido2.CredentialMakeResult();
            var result = new EnrollAuthenticatorDeviceCommandResult(credentialMakeResult, TestVariables.AuthenticatorDeviceId);

            Assert.Equal(credentialMakeResult, result.CredentialMakeResult);
        }
    }
}