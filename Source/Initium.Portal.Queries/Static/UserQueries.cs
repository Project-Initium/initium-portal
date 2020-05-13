// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Queries.Contracts.Static;
using Initium.Portal.Queries.Static.Models;
using Initium.Portal.Queries.Static.Models.User;
using Initium.Portal.Queries.Static.TransferObjects;
using Initium.Portal.Queries.Static.TransferObjects.User;
using MaybeMonad;

namespace Initium.Portal.Queries.Static
{
    public sealed class UserQueries : IUserQueries
    {
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;
        private readonly IConnectionProvider _connectionProvider;

        public UserQueries(ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider, IConnectionProvider connectionProvider)
        {
            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ?? throw new ArgumentNullException(nameof(currentAuthenticatedUserProvider));
            this._connectionProvider = connectionProvider;
        }

        public async Task<StatusCheckModel> CheckForPresenceOfAnyUser()
        {
            var command = new CommandDefinition(
                "[Portal].[uspCheckForPresenceOfAnyUser]",
                commandType: CommandType.StoredProcedure);

            using var connection = this._connectionProvider.GetConnection();
            connection.Open();

            var res = await connection.QueryAsync<PresenceCheckDto<Guid>>(command);
            var dataItems = res as PresenceCheckDto<Guid>[] ?? res.ToArray();
            return new StatusCheckModel(dataItems.Length > 0);
        }

        public async Task<Maybe<ProfileModel>> GetProfileForCurrentUser()
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return Maybe<ProfileModel>.Nothing;
            }

            var parameters = new DynamicParameters();
            parameters.Add("userId", currentUser.Value.UserId, DbType.Guid);

            var command = new CommandDefinition(
                "[Portal].[uspGetProfileForCurrentUser]",
                parameters,
                commandType: CommandType.StoredProcedure);

            using var connection = this._connectionProvider.GetConnection();
            connection.Open();

            var res = await connection.QueryAsync<ProfileDto>(command);
            var dataItems = res as ProfileDto[] ?? res.ToArray();
            return dataItems.Length != 1 ? Maybe<ProfileModel>.Nothing : Maybe.From(new ProfileModel(dataItems.First().FirstName, dataItems.First().LastName));
        }

        public async Task<StatusCheckModel> CheckForPresenceOfUserByEmailAddress(string emailAddress)
        {
            var parameters = new DynamicParameters();
            parameters.Add("emailAddress", emailAddress, DbType.String);

            var command = new CommandDefinition(
                "[Portal].[uspCheckForPresenceOfUserByEmailAddress]",
                parameters,
                commandType: CommandType.StoredProcedure);

            using var connection = this._connectionProvider.GetConnection();
            connection.Open();

            var res = await connection.QueryAsync<PresenceCheckDto<Guid>>(command);
            var dataItems = res as PresenceCheckDto<Guid>[] ?? res.ToArray();
            return new StatusCheckModel(dataItems.Length > 0);
        }

        public async Task<Maybe<DetailedUserModel>> GetDetailsOfUserById(Guid userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("userId", userId, DbType.Guid);

            var command = new CommandDefinition(
                @"[Portal].[uspGetDetailsOfUserById]",
                parameters,
                commandType: CommandType.StoredProcedure);

            using var connection = this._connectionProvider.GetConnection();
            connection.Open();

            var res = await connection.QueryMultipleAsync(command);
            var detailedUseResult = await res.ReadAsync<DetailedUserDto>();
            var dtos = detailedUseResult as DetailedUserDto[] ?? detailedUseResult.ToArray();
            if (dtos.Length != 1)
            {
                return Maybe<DetailedUserModel>.Nothing;
            }

            var entity = dtos.Single();

            var resourceResult = await res.ReadAsync<Guid>();

            return Maybe.From(
                new DetailedUserModel(entity.Id, entity.EmailAddress, entity.FirstName, entity.LastName, entity.IsLockable, entity.WhenCreated, entity.WhenLastAuthenticated, entity.WhenLocked, entity.IsAdmin, resourceResult.ToList(), entity.WhenDisabled));
        }

        public async Task<Maybe<SystemProfileModel>> GetSystemProfileByUserId(Guid userId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("userId", userId, DbType.Guid);

            var command = new CommandDefinition(
                "[Portal].[uspGetSystemProfileByUserId]",
                parameters,
                commandType: CommandType.StoredProcedure);

            using var connection = this._connectionProvider.GetConnection();
            connection.Open();

            var res = await connection.QueryMultipleAsync(command);
            var profileResult = await res.ReadAsync<SystemProfileDto>();
            var dtos = profileResult as SystemProfileDto[] ?? profileResult.ToArray();
            if (dtos.Length != 1)
            {
                return Maybe<SystemProfileModel>.Nothing;
            }

            var entity = dtos.Single();

            var resourceResult = await res.ReadAsync<string>();

            return Maybe.From(
                new SystemProfileModel(entity.EmailAddress, entity.FirstName, entity.LastName, entity.IsAdmin, resourceResult.ToList()));
        }

        public async Task<StatusCheckModel> CheckForPresenceOfAuthAppForCurrentUser()
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                return new StatusCheckModel(true);
            }

            var parameters = new DynamicParameters();
            parameters.Add("userId", currentUserMaybe.Value.UserId, DbType.Guid);

            var command = new CommandDefinition(
                "[Portal].[uspCheckForPresenceOfAuthAppForCurrentUser]",
                parameters,
                commandType: CommandType.StoredProcedure);

            using var connection = this._connectionProvider.GetConnection();
            connection.Open();

            var res = await connection.QueryAsync<PresenceCheckDto<Guid>>(command);
            var dtos = res as PresenceCheckDto<Guid>[] ?? res.ToArray();
            return new StatusCheckModel(dtos.Length > 0);
        }

        public async Task<Maybe<List<DeviceInfo>>> GetDeviceInfoForCurrentUser()
        {
            var currentUserMaybe = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUserMaybe.HasNoValue)
            {
                return Maybe<List<DeviceInfo>>.Nothing;
            }

            var parameters = new DynamicParameters();
            parameters.Add("userId", currentUserMaybe.Value.UserId, DbType.Guid);

            var command = new CommandDefinition(
                "[Portal].[uspGetDeviceInfoForCurrentUser]",
                parameters,
                commandType: CommandType.StoredProcedure);

            using var connection = this._connectionProvider.GetConnection();
            connection.Open();

            var res = await connection.QueryAsync<DeviceInfoDto>(command);
            var dtos = res as DeviceInfoDto[] ?? res.ToArray();
            if (dtos.Length == 0)
            {
                return Maybe<List<DeviceInfo>>.Nothing;
            }

            return Maybe.From(new List<DeviceInfo>(dtos.Select(x =>
                new DeviceInfo(x.Id, x.Name, x.WhenEnrolled, x.WhenLastUsed))));
        }
    }
}