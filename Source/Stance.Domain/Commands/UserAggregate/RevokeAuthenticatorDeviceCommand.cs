// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using MediatR;
using ResultMonad;
using Stance.Core.Domain;

namespace Stance.Domain.Commands.UserAggregate
{
    public class RevokeAuthenticatorDeviceCommand : IRequest<ResultWithError<ErrorData>>
    {
        public RevokeAuthenticatorDeviceCommand(Guid deviceId)
        {
            this.DeviceId = deviceId;
        }

        public Guid DeviceId { get; }
    }
}