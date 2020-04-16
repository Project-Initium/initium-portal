// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;
using Stance.Domain.CommandResults.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandResults.UserAggregate
{
    public class DeviceMfaRequestCommandResultTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var assertionOptions = new AssertionOptions();
            var result = new DeviceMfaRequestCommandResult(assertionOptions);

            Assert.Equal(assertionOptions, result.AssertionOptions);
        }
    }
}