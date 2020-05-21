// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Constants;
using Initium.Portal.Domain.Commands.SystemAlertAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.Commands.SystemAlertAggregate
{
    public class UpdateSystemAlertCommandTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var command = new UpdateSystemAlertCommand(
                TestVariables.SystemAlertId,
                "message",
                SystemAlertType.Critical,
                TestVariables.Now.AddDays(-1),
                TestVariables.Now.AddDays(1));

            Assert.Equal(TestVariables.SystemAlertId, command.SystemAlertId);
            Assert.Equal("message", command.Message);
            Assert.Equal(SystemAlertType.Critical, command.Type);
            Assert.Equal(TestVariables.Now.AddDays(-1), command.WhenToShow);
            Assert.Equal(TestVariables.Now.AddDays(1), command.WhenToHide);
        }
    }
}