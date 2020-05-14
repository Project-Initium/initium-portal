// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Domain.CommandResults.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandResults.UserAggregate
{
    public class CreateUserCommandResultTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var commandResult = new CreateUserCommandResult(TestVariables.UserId);
            Assert.Equal(TestVariables.UserId, commandResult.UserId);
        }
    }
}