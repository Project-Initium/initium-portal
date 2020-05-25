// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Domain.CommandResults.SystemAlertAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandResults.SystemAlertAggregate
{
    public class CreateNewSystemAlertCommandResultTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var commandResult = new CreateNewSystemAlertCommandResult(TestVariables.SystemAlertId);

            Assert.Equal(TestVariables.SystemAlertId, commandResult.SystemAlertId);
        }
    }
}