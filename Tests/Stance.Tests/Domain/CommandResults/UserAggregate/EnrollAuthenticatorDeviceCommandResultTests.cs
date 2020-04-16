// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;
using Stance.Domain.CommandResults.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandResults.UserAggregate
{
    public class EnrollAuthenticatorDeviceCommandResultTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var credentialMakeResult = new Fido2.CredentialMakeResult();
            var result = new EnrollAuthenticatorDeviceCommandResult(credentialMakeResult);

            Assert.Equal(credentialMakeResult, result.CredentialMakeResult);
        }
    }
}