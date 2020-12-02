// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.AggregatesModel.TenantAggregate;
using MaybeMonad;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Infrastructure.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly DataContext _context;

        public TenantRepository(DataContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => this._context;

        public ITenant Add(ITenant tenant)
        {
            var entity = tenant as Tenant;
            if (entity == null)
            {
                throw new ArgumentException(nameof(tenant));
            }

            return this._context.Tenants.Add(entity).Entity;
        }

        public void Update(ITenant tenant)
        {
            var entity = tenant as Tenant;
            if (entity == null)
            {
                throw new ArgumentException(nameof(tenant));
            }

            this._context.Tenants.Update(entity);
        }

        public async Task<Maybe<ITenant>> Find(Guid tenantId, CancellationToken cancellationToken = default)
        {
            var tenant = await this._context.Tenants
                .Include(x => x.TenantFeatures)
                .SingleOrDefaultAsync(x => x.Id == tenantId, cancellationToken);
            await this.Refresh(tenant);
            return Maybe.From<ITenant>(tenant);
        }

        private async Task Refresh(ITenant tenant)
        {
            if (tenant != null)
            {
                await this._context.Entry(tenant).ReloadAsync();
            }
        }
    }
}