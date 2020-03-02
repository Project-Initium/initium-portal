// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Domain.CommandResults.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandResults.UserAggregate
{
    public class CreateUserCommandResultTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var id = Guid.NewGuid();
            var commandResult = new CreateUserCommandResult(id);
            Assert.Equal(id, commandResult.UserId);
        }
    }
}