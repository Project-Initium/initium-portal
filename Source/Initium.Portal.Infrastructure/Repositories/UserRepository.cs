// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Core.Database;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using MaybeMonad;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Infrastructure.Repositories
{
    public sealed class UserRepository : IUserRepository
    {
        private readonly GenericDataContext _context;

        public UserRepository(GenericDataContext context)
        {
            this._context = context;
        }

        public IUnitOfWork UnitOfWork => this._context;

        public void Update(IUser user)
        {
            var entity = user as User;
            if (entity == null)
            {
                throw new ArgumentException(nameof(user));
            }

            this._context.Set<User>().Update(entity);
        }

        public async Task<Maybe<IUser>> Find(Guid userId, CancellationToken cancellationToken = default)
        {
            var user = await this._context.Set<User>()
                .Include(x => x.SecurityTokenMappings)
                .Include(x => x.AuthenticatorApps)
                .Include(x => x.AuthenticatorDevices)
                .Include(x => x.PasswordHistories)
                .Include(x => x.UserNotifications)
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

            return this._context.Set<User>().Add(entity).Entity;
        }

        public async Task<Maybe<IUser>> FindByEmailAddress(string emailAddress, CancellationToken cancellationToken = default)
        {
            var user = await this._context.Set<User>()
                .Include(x => x.SecurityTokenMappings)
                .Include(x => x.AuthenticatorApps)
                .Include(x => x.AuthenticatorDevices)
                .Include(x => x.PasswordHistories)
                .Include(x => x.UserNotifications)
                .SingleOrDefaultAsync(x => x.EmailAddress == emailAddress, cancellationToken);
            await this.Refresh(user);
            return Maybe.From<IUser>(user);
        }

        public async Task<Maybe<IUser>> FindByUserBySecurityToken(Guid tokenId, DateTime expiryDate, CancellationToken cancellationToken = default)
        {
            var user = await this._context.Set<User>()
                .Include(x => x.SecurityTokenMappings)
                .Include(x => x.AuthenticatorApps)
                .Include(x => x.AuthenticatorDevices)
                .Include(x => x.PasswordHistories)
                .Include(x => x.UserNotifications)
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