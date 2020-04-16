// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Stance.Domain.Commands.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.Commands.UserAggregate
{
    public class ValidateAppMfaCodeAgainstCurrentUserCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var command = new ValidateAppMfaCodeAgainstCurrentUserCommand("code");
            Assert.Equal("code", command.Code);
        }
    }
}