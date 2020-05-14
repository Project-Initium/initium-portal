// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Core
{
    public interface ISystemUser
    {
        Guid UserId { get; }
    }
}