// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Constants;

namespace Initium.Portal.Core
{
    public class UnauthenticatedUser : ISystemUser
    {
        public UnauthenticatedUser(Guid userId, MfaProvider setupMfaProviders)
        {
            this.UserId = userId;
            this.SetupMfaProviders = setupMfaProviders;
        }

        public Guid UserId { get; }

        public MfaProvider SetupMfaProviders { get; }
    }
}