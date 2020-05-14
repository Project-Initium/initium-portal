// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;

namespace Initium.Portal.Domain.CommandResults.UserAggregate
{
    public class DeviceMfaRequestCommandResult
    {
        public DeviceMfaRequestCommandResult(AssertionOptions assertionOptions)
        {
            this.AssertionOptions = assertionOptions;
        }

        public AssertionOptions AssertionOptions { get; }
    }
}