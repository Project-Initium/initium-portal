// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Queries;
using Initium.Portal.Queries.Entities;
using Initium.Portal.Queries.Models;
using Initium.Portal.Queries.Models.Tenant;
using MaybeMonad;

namespace Initium.Portal.Queries.Contracts
{
    public interface ITenantQueryService : IQueryService<TenantDto>
    {
        Task<StatusCheckModel> CheckForPresenceOfTenantByIdentifier(string identifier, CancellationToken cancellationToken = default);

        Task<Maybe<TenantMetadata>> GetTenantMetadataById(Guid id, CancellationToken cancellationToken = default);

        Task<Maybe<TenantMetadata>> GetTenantMetadataByIdentifier(string identifier, CancellationToken cancellationToken = default);
    }
}