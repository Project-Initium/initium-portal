// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Queries.Management.Contracts;
using Initium.Portal.Queries.Management.Entities;
using Initium.Portal.Queries.Models;
using Initium.Portal.Queries.Models.Tenant;
using MaybeMonad;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries.Management
{
    public class TenantQueryService : ITenantQueryService
    {
        private readonly IManagementQueryContext _queryContext;

        public TenantQueryService(IManagementQueryContext queryContext)
        {
            this._queryContext = queryContext;
        }

        public IQueryable<TenantReadEntity> QueryableEntity => this._queryContext.Tenants;

        public async Task<StatusCheckModel> CheckForPresenceOfTenantByIdentifier(string identifier, CancellationToken cancellationToken = default)
        {
            return new StatusCheckModel(await this._queryContext.Tenants.AnyAsync(x => x.Identifier == identifier, cancellationToken: cancellationToken));
        }

        public async Task<Maybe<TenantMetadata>> GetTenantMetadataById(Guid id, CancellationToken cancellationToken = default)
        {
            var data = await this._queryContext.Tenants.FirstOrDefaultAsync(x => x.Id == id, cancellationToken: cancellationToken);
            return data == null ? Maybe<TenantMetadata>.Nothing : new TenantMetadata(data.Id, data.Identifier, data.Name, data.ConnectionString, data.WhenDisabled);
        }
    }
}