// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.MultiTenant;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Infrastructure.Tenant
{
    public class TenantDataContext : CoreDataContext, ITenantDataContext
    {
        public TenantDataContext(FeatureBasedTenantInfo tenantInfo, IMediator mediator)
            : base(tenantInfo, mediator)
        {
        }

        protected internal TenantDataContext(DbContextOptions<CoreDataContext> options, IMediator mediator, FeatureBasedTenantInfo tenantInfo)
            : base(options, mediator, tenantInfo)
        {
        }
    }
}