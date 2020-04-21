// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib.Objects;

namespace Stance.Web.Controllers.Api.AuthDevice
{
    public class InitialAuthDeviceRegistrationRequest
    {
        public string Name { get; set; }

        public AuthenticatorAttachment AuthenticatorAttachment { get; set; }
    }
}