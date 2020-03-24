// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;

namespace Stance.Domain.CommandResults.UserAggregate
{
    public class EnrollAuthenticatorDeviceCommandResult
    {
        public EnrollAuthenticatorDeviceCommandResult(Fido2.CredentialMakeResult credentialMakeResult)
        {
            this.CredentialMakeResult = credentialMakeResult;
        }

        public Fido2.CredentialMakeResult CredentialMakeResult { get; }
    }
}