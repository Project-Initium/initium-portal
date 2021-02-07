// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Database;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Entities;
using Initium.Portal.Queries.Models;
using Initium.Portal.Queries.Models.User;
using MaybeMonad;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries
{
    public sealed class UserQueryService : IUserQueryService
    {
        private readonly GenericDataContext _context;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        public UserQueryService(ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, GenericDataContext context)
        {
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider;
            this._context = context;
        }

        public IQueryable<UserReadEntity> QueryableEntity => this._context.Set<UserReadEntity>();

        public async Task<StatusCheckModel> CheckForPresenceOfAnyUser()
        {
            return new StatusCheckModel(await this.QueryableEntity.AnyAsync());
        }

        public async Task<Maybe<ProfileModel>> GetProfileForCurrentUser()
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return Maybe<ProfileModel>.Nothing;
            }

            var data = await this.QueryableEntity
                .Where(x => x.Id == currentUser.Value.UserId)
                .Select(x =>
                new
                {
                    x.FirstName,
                    x.LastName,
                }).SingleOrDefaultAsync();

            return data == null
                ? Maybe<ProfileModel>.Nothing
                : Maybe.From(new ProfileModel(data.FirstName, data.LastName));
        }

        public async Task<StatusCheckModel> CheckForPresenceOfUserByEmailAddress(string emailAddress)
        {
            return new StatusCheckModel(await this.QueryableEntity.AnyAsync(user => user.EmailAddress == emailAddress));
        }

        public async Task<Maybe<DetailedUserModel>> GetDetailsOfUserById(Guid userId)
        {
            var data = await this.QueryableEntity.Where(user => user.Id == userId).Select(user =>
                new
                {
                    user.Id,
                    user.EmailAddress,
                    user.IsLockable,
                    user.WhenCreated,
                    user.WhenLastAuthenticated,
                    user.WhenLocked,
                    user.IsAdmin,
                    Resources = user.UserRoles.SelectMany(role => role.Role.RoleResources.Select(resource => resource.ResourceId)),
                    user.WhenDisabled,
                    user.FirstName,
                    user.LastName,
                }).SingleOrDefaultAsync();

            return data == null ? Maybe<DetailedUserModel>.Nothing : Maybe.From(
                new DetailedUserModel(data.Id, data.EmailAddress, data.FirstName, data.LastName,
                    data.IsLockable, data.WhenCreated, data.WhenLastAuthenticated, data.WhenLocked,
                    data.IsAdmin, data.Resources.ToList(), data.WhenDisabled));
        }

        public async Task<Maybe<SystemProfileModel>> GetSystemProfileByUserId(Guid userId)
        {
            var data = await this.QueryableEntity.Where(user => user.Id == userId).Select(user =>
                new
                {
                    user.EmailAddress,
                    user.IsAdmin,
                    Resources = user.UserRoles.SelectMany(role => role.Role.RoleResources.Select(resource => resource.Resource.NormalizedName)),
                    user.FirstName,
                    user.LastName,
                }).SingleOrDefaultAsync();

            return data == null ? Maybe<SystemProfileModel>.Nothing : Maybe.From(
                new SystemProfileModel(data.EmailAddress, data.FirstName, data.LastName, data.IsAdmin,
                    data.Resources.ToList()));
        }

        public async Task<StatusCheckModel> CheckForPresenceOfAuthAppForCurrentUser()
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            return currentUserMaybe.HasNoValue ? new StatusCheckModel(false) : new StatusCheckModel(await this.QueryableEntity.AnyAsync(x => x.Id == currentUserMaybe.Value.UserId && x.AuthenticatorApps.Any()));
        }

        public async Task<Maybe<List<DeviceInfo>>> GetDeviceInfoForCurrentUser()
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                return Maybe<List<DeviceInfo>>.Nothing;
            }

            var data = await this.QueryableEntity
                .Include(user => user.AuthenticatorDevices)
                .Where(user => user.Id == currentUserMaybe.Value.UserId)
                .SelectMany(user => user.AuthenticatorDevices
                    .Select(device => new
                    {
                        device.Id,
                        device.Name,
                        device.WhenEnrolled,
                        device.WhenLastUsed,
                    })).ToListAsync();

            return data == null || data.Count == 0 ? Maybe<List<DeviceInfo>>.Nothing : Maybe.From(new List<DeviceInfo>(data.Select(x =>
                new DeviceInfo(x.Id, x.Name, x.WhenEnrolled, x.WhenLastUsed))));
        }
    }
}