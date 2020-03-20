// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Microsoft.EntityFrameworkCore;
using Stance.Core.Contracts.Domain;
using Stance.Domain.AggregatesModel.UserAggregate;

namespace Stance.Infrastructure.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            this._context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IUnitOfWork UnitOfWork => this._context;

        public void Update(IUser user)
        {
            var entity = user as User;
            if (entity == null)
            {
                throw new ArgumentException(nameof(user));
            }

            this._context.Users.Update(entity);
        }

        public async Task<Maybe<IUser>> Find(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await this._context.Users
                .Include(x => x.SecurityTokenMappings)
                .Include(x => x.AuthenticatorApps)
                .SingleOrDefaultAsync(x => x.Id == userId, cancellationToken);
            await this.Refresh(user);
            return Maybe.From<IUser>(user);
        }

        public IUser Add(IUser user)
        {
            var entity = user as User;
            if (entity == null)
            {
                throw new ArgumentException(nameof(user));
            }

            return this._context.Users.Add(entity).Entity;
        }

        public async Task<Maybe<IUser>> FindByEmailAddress(string emailAddress, CancellationToken cancellationToken = default)
        {
            var user = await this._context.Users
                .Include(x => x.SecurityTokenMappings)
                .Include(x => x.AuthenticatorApps)
                .SingleOrDefaultAsync(x => x.EmailAddress == emailAddress, cancellationToken);
            await this.Refresh(user);
            return Maybe.From<IUser>(user);
        }

        public async Task<Maybe<IUser>> FindByUserBySecurityToken(Guid tokenId, DateTime expiryDate, CancellationToken cancellationToken = default)
        {
            var user = await this._context.Users
                .Include(x => x.SecurityTokenMappings)
                .Include(x => x.AuthenticatorApps)
                .SingleOrDefaultAsync(x => x.SecurityTokenMappings.Any(y => y.Id == tokenId && y.WhenExpires > expiryDate), cancellationToken);
            await this.Refresh(user);
            return Maybe.From<IUser>(user);
        }

        private async Task Refresh(IUser user)
        {
            if (user != null)
            {
                await this._context.Entry(user).ReloadAsync();
            }
        }
    }
}