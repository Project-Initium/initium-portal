// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Domain.Commands.SystemAlertAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.Commands.SystemAlertAggregate
{
    public class DeleteSystemAlertCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var command = new DeleteSystemAlertCommand(TestVariables.SystemAlertId);

            Assert.Equal(TestVariables.SystemAlertId, command.SystemAlertId);
        }
    }
}