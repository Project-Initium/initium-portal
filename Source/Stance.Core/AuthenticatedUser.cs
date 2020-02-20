// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Stance.Core
{
    public sealed class AuthenticatedUser
    {
        public AuthenticatedUser(Guid userId)
        {
            this.UserId = userId;
        }

        public Guid UserId { get; }
    }
}