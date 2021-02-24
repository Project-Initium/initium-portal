// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using ProtoBuf;

namespace Initium.Portal.Domain.EventHandlers.Models
{
    [ProtoContract(SkipConstructor = true)]
    public class SecurityToken
    {
        public SecurityToken(Guid token, long whenExpires)
        {
            this.Token = token;
            this.WhenExpires = whenExpires;
        }

        [ProtoMember(1)]
        public Guid Token { get; private set; }

        [ProtoMember(2)]
        public long WhenExpires { get; private set; }
    }
}