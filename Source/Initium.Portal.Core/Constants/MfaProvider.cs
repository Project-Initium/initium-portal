// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Core.Constants
{
    [Flags]
    public enum MfaProvider
    {
        None = 0,
        Email = 1,
        App = 2,
        Device = 4,
    }
}