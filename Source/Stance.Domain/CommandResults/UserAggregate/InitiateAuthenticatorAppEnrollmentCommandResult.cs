// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

namespace Stance.Domain.CommandResults.UserAggregate
{
    public class InitiateAuthenticatorAppEnrollmentCommandResult
    {
        public InitiateAuthenticatorAppEnrollmentCommandResult(string sharedKey)
        {
            this.SharedKey = sharedKey;
        }

        public string SharedKey { get; }
    }
}