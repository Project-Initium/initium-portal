// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Queries;
using Initium.Portal.Queries.Management.Entities;
using Initium.Portal.Queries.Management.Tenant;
using Initium.Portal.Queries.Models;
using MaybeMonad;

namespace Initium.Portal.Queries.Management.Contracts
{
    public interface ITenantQueryService : IQueryService<TenantReadEntity>
    {
        Task<Maybe<TenantMetadata>> GetTenantMetadataById(Guid id, CancellationToken cancellationToken = default);
    }
}