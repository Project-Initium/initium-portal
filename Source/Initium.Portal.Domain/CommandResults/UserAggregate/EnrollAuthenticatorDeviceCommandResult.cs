// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Fido2NetLib;

namespace Initium.Portal.Domain.CommandResults.UserAggregate
{
    public class EnrollAuthenticatorDeviceCommandResult
    {
        public EnrollAuthenticatorDeviceCommandResult(Fido2.CredentialMakeResult credentialMakeResult, Guid deviceId)
        {
            this.CredentialMakeResult = credentialMakeResult;
            this.DeviceId = deviceId;
        }

        public Fido2.CredentialMakeResult CredentialMakeResult { get; }

        public Guid DeviceId { get; }
    }
}