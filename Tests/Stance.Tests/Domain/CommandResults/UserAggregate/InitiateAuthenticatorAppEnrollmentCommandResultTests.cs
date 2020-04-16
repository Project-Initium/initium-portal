// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Stance.Domain.CommandResults.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandResults.UserAggregate
{
    public class InitiateAuthenticatorAppEnrollmentCommandResultTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var initiateAuthenticatorAppEnrollmentCommandResult =
                new InitiateAuthenticatorAppEnrollmentCommandResult("shared-key");
            Assert.Equal("shared-key", initiateAuthenticatorAppEnrollmentCommandResult.SharedKey);
        }
    }
}