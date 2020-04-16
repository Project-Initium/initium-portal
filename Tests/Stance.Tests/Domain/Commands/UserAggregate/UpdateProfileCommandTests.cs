// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Stance.Domain.Commands.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.Commands.UserAggregate
{
    public class UpdateProfileCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var updateProfileCommand = new UpdateProfileCommand("first-name", "last-name");

            Assert.Equal("first-name", updateProfileCommand.FirstName);
            Assert.Equal("last-name", updateProfileCommand.LastName);
        }
    }
}