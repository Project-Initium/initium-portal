// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandResults.UserAggregate
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