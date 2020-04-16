// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;
using Stance.Domain.Commands.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.Commands.UserAggregate
{
    public class ValidateDeviceMfaAgainstCurrentUserCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var authenticatorAssertionRawResponse = new AuthenticatorAssertionRawResponse();
            var assertionOptions = new AssertionOptions();
            var command =
                new ValidateDeviceMfaAgainstCurrentUserCommand(authenticatorAssertionRawResponse, assertionOptions);
            Assert.Equal(authenticatorAssertionRawResponse, command.AuthenticatorAssertionRawResponse);
            Assert.Equal(assertionOptions, command.AssertionOptions);
        }
    }
}