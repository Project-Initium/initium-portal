// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Stance.Domain.Commands.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.Commands.UserAggregate
{
    public class RevokeAuthenticatorDeviceCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var command = new RevokeAuthenticatorDeviceCommand(TestVariables.AuthenticatorDeviceId);
            Assert.Equal(TestVariables.AuthenticatorDeviceId, command.DeviceId);
        }
    }
}