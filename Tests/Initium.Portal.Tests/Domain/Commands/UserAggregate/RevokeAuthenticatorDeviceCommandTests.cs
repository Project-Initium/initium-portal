﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Domain.Commands.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.Commands.UserAggregate
{
    public class RevokeAuthenticatorDeviceCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var command = new RevokeAuthenticatorDeviceCommand(TestVariables.AuthenticatorDeviceId, "password");
            Assert.Equal(TestVariables.AuthenticatorDeviceId, command.DeviceId);
            Assert.Equal("password", command.Password);
        }
    }
}