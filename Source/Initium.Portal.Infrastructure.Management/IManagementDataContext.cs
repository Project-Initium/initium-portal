// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Common.Domain.AggregatesModel.TenantAggregate;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Infrastructure.Admin
{
    public interface IManagementDataContext : ICoreDataContext
    {
        DbSet<Tenant> Tenants { get; set; }
    }
}