﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Domain.Commands.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.Commands.UserAggregate
{
    public class PasswordResetCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var passwordResetCommand = new PasswordResetCommand(
                TestVariables.SecurityTokenMappingId, "new-password");

            Assert.Equal(TestVariables.SecurityTokenMappingId, passwordResetCommand.Token);
            Assert.Equal("new-password", passwordResetCommand.NewPassword);
        }
    }
}