// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Domain.Commands.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.Commands.UserAggregate
{
    public class CreateInitialUserCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var command = new CreateInitialUserCommand("email-address", "password", "first-name", "last-name");
            Assert.Equal("email-address", command.EmailAddress);
            Assert.Equal("password", command.Password);
            Assert.Equal("first-name", command.FirstName);
            Assert.Equal("last-name", command.LastName);
        }
    }
}