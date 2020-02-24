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
            var firstName = new string('*', 5);
            var lastName = new string('*', 6);

            var updateProfileCommand = new UpdateProfileCommand(firstName, lastName);

            Assert.Equal(firstName, updateProfileCommand.FirstName);
            Assert.Equal(lastName, updateProfileCommand.LastName);
        }
    }
}