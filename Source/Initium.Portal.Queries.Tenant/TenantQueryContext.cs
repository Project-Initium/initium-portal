﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Core.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Initium.Portal.Queries.Tenant
{
    public class TenantQueryContext : CoreQueryContext, ITenantQueryContext
    {
        public TenantQueryContext(FeatureBasedTenantInfo tenantInfo, IOptions<MultiTenantSettings> multiTenantSettings)
            : base(tenantInfo, multiTenantSettings)
        {
        }

        internal TenantQueryContext(DbContextOptions<CoreQueryContext> options, FeatureBasedTenantInfo tenantInfo, IOptions<MultiTenantSettings> multiTenantSettings)
            : base(options, tenantInfo, multiTenantSettings)
        {
        }
    }
}