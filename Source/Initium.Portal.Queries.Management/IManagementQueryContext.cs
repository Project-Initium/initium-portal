// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Queries.Management.Entities;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries.Management
{
    public interface IManagementQueryContext : ICoreQueryContext
    {
        DbSet<TenantReadEntity> Tenants { get; set; }
    }
}