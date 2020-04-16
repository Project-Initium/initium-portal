// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib.Objects;
using Stance.Domain.Commands.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.Commands.UserAggregate
{
    public class InitiateAuthenticatorDeviceEnrollmentCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var command = new InitiateAuthenticatorDeviceEnrollmentCommand(AuthenticatorAttachment.CrossPlatform);
            Assert.Equal(AuthenticatorAttachment.CrossPlatform, command.AuthenticatorAttachment);
        }
    }
}