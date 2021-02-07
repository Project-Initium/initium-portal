// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Database;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using MaybeMonad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Initium.Portal.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly GenericDataContext _context;

        public RoleRepository(GenericDataContext context)
        {
            this._context = context;
        }

        public IUnitOfWork UnitOfWork => this._context;

        public IRole Add(IRole role)
        {
            var entity = role as Role;
            if (entity == null)
            {
                throw new ArgumentException(nameof(role));
            }

            return this._context.Set<Role>().Add(entity).Entity;
        }

        public async Task<Maybe<IRole>> Find(Guid id, CancellationToken cancellationToken = default)
        {
            var role = await this.WithRelatedData().SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
            await this.Refresh(role);
            return Maybe.From<IRole>(role);
        }

        public void Update(IRole role)
        {
            var entity = role as Role;
            if (entity == null)
            {
                throw new ArgumentException(nameof(role));
            }

            this._context.Set<Role>().Update(entity);
        }

        public void Delete(IRole role)
        {
            var entity = role as Role;
            if (entity == null)
            {
                throw new ArgumentException(nameof(role));
            }

            this._context.Set<Role>().Remove(entity);
        }

        private async Task Refresh(IRole role)
        {
            if (role != null)
            {
                await this._context.Entry(role).ReloadAsync();
            }
        }

        private IIncludableQueryable<Role, IReadOnlyList<RoleResource>> WithRelatedData()
        {
            return this._context.Set<Role>().Include(x => x.RoleResources);
        }
    }
}