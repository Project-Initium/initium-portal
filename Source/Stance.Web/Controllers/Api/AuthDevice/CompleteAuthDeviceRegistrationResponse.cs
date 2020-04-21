// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Fido2NetLib;

namespace Stance.Web.Controllers.Api.AuthDevice
{
    public class CompleteAuthDeviceRegistrationResponse
    {
        public CompleteAuthDeviceRegistrationResponse()
        {
            this.DeviceId = Guid.Empty;
            this.Result = new Fido2.CredentialMakeResult { Status = "error" };
        }

        public CompleteAuthDeviceRegistrationResponse(Fido2.CredentialMakeResult result, Guid deviceId, string name)
        {
            this.Result = result;
            this.DeviceId = deviceId;
            this.Name = name;
        }

        public Fido2.CredentialMakeResult Result { get; }

        public Guid DeviceId { get; }

        public string Name { get; }
    }
}