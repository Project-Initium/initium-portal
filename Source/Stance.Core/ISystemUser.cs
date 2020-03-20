// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Stance.Core
{
    public interface ISystemUser
    {
        Guid UserId { get; }
    }
}