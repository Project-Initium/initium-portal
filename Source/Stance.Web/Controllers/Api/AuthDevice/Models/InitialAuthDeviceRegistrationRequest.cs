// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib.Objects;

namespace Stance.Web.Controllers.Api.AuthDevice.Models
{
    public class InitialAuthDeviceRegistrationRequest
    {
        public AuthenticatorAttachment AuthenticatorAttachment { get; set; }
    }
}