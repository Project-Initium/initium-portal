// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Stance.Web.Controllers.Api.AuthDevice.Models
{
    public class RevokeDeviceRequest
    {
        public Guid DeviceId { get; set; }

        public string Password { get; set; }
    }
}