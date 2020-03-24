// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib;

namespace Stance.Web.Controllers.Api.AuthDevice
{
    public class CompleteAuthDeviceRegistrationRequest
    {
        public AuthenticatorAttestationRawResponse AttestationResponse { get; set; }

        public string Name { get; set; }
    }
}