// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Stance.Core.Contracts.Domain;
using Stance.Domain.AggregatesModel.RoleAggregate;

namespace Stance.Infrastructure.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly DataContext _context;

        public RoleRepository(DataContext context)
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

            return this._context.Roles.Add(entity).Entity;
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

            this._context.Roles.Update(entity);
        }

        public void Delete(IRole role)
        {
            var entity = role as Role;
            if (entity == null)
            {
                throw new ArgumentException(nameof(role));
            }

            this._context.Roles.Remove(entity);
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
            return this._context.Roles.Include(x => x.RoleResources);
        }
    }
}