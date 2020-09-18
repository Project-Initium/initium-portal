// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Constants;

namespace Initium.Portal.Core.Settings
{
    public class MultiTenantSettings
    {
        public string DefaultTenantConnectionString { get; set; }

        public Guid DefaultTenantId { get; set; }

        public MultiTenantType MultiTenantType { get; set; }
    }
}