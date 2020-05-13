// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Fido2NetLib.Objects;

namespace Initium.Portal.Web.Controllers.Api.AuthDevice.Models
{
    public class InitialAuthDeviceRegistrationRequest
    {
        public AuthenticatorAttachment AuthenticatorAttachment { get; set; }
    }
}