// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MaybeMonad;
using Microsoft.Extensions.Options;
using Stance.Core.Contracts;
using Stance.Queries.Contracts;
using Stance.Queries.Models;
using Stance.Queries.Models.User;
using Stance.Queries.TransferObjects;
using Stance.Queries.TransferObjects.User;

namespace Stance.Queries
{
    public sealed class UserQueries : IUserQueries
    {
        private readonly QuerySettings _querySettings;
        private readonly ICurrentAuthenticatedUserProvider _currentAuthenticatedUserProvider;

        public UserQueries(IOptions<QuerySettings> querySettings, ICurrentAuthenticatedUserProvider currentAuthenticatedUserProvider)
        {
            if (querySettings == null)
            {
                throw new ArgumentNullException(nameof(querySettings));
            }

            this._currentAuthenticatedUserProvider = currentAuthenticatedUserProvider ?? throw new ArgumentNullException(nameof(currentAuthenticatedUserProvider));

            this._querySettings = querySettings.Value;
        }

        public async Task<StatusCheckModel> CheckForPresenceOfAnyUser(CancellationToken cancellationToken = default)
        {
            var command = new CommandDefinition(
                "select top 1 id from [identity].[user]",
                cancellationToken: cancellationToken);

            using var connection = new SqlConnection(this._querySettings.ConnectionString);
            connection.Open();

            var res = await connection.QueryAsync<PresenceCheckDto<Guid>>(command);
            var dataItems = res as PresenceCheckDto<Guid>[] ?? res.ToArray();
            return new StatusCheckModel(dataItems.Length > 0);
        }

        public async Task<Maybe<ProfileModel>> GetProfileForCurrentUser(CancellationToken cancellationToken = default)
        {
            var currentUser = this._currentAuthenticatedUserProvider.CurrentAuthenticatedUser;
            if (currentUser.HasNoValue)
            {
                return Maybe<ProfileModel>.Nothing;
            }

            var parameters = new DynamicParameters();
            parameters.Add("userId", currentUser.Value.UserId, DbType.Guid);

            var command = new CommandDefinition(
                "select firstName, lastName from [identity].[profile] where userId = @userId",
                parameters,
                cancellationToken: cancellationToken);

            using var connection = new SqlConnection(this._querySettings.ConnectionString);
            connection.Open();

            var res = await connection.QueryAsync<ProfileDto>(command);
            var dataItems = res as ProfileDto[] ?? res.ToArray();
            return dataItems.Length != 1 ? Maybe<ProfileModel>.Nothing : Maybe.From(new ProfileModel(dataItems.First().FirstName, dataItems.First().LastName));
        }

        public async Task<StatusCheckModel> CheckForPresenceOfUserByEmailAddress(string emailAddress, CancellationToken cancellationToken = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("emailAddress", emailAddress, DbType.String);

            var command = new CommandDefinition(
                "select top 1 id from [identity].[user] where emailAddress = @emailAddress",
                parameters,
                cancellationToken: cancellationToken);

            using var connection = new SqlConnection(this._querySettings.ConnectionString);
            connection.Open();

            var res = await connection.QueryAsync<PresenceCheckDto<Guid>>(command);
            var dataItems = res as PresenceCheckDto<Guid>[] ?? res.ToArray();
            return new StatusCheckModel(dataItems.Length > 0);
        }

        public async Task<Maybe<DetailedUserModel>> GetDetailsOfUserById(Guid userId, CancellationToken cancellationToken = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("userId", userId, DbType.Guid);

            var command = new CommandDefinition(
                "select u.id, u.emailAddress, p.firstName, p.lastName, u.isLockable, u.whenCreated, u.whenLastAuthenticated, u.whenLocked from [identity].[user] u join [identity].[profile] p on u.Id = p.UserId where id = @userId",
                parameters,
                cancellationToken: cancellationToken);

            using var connection = new SqlConnection(this._querySettings.ConnectionString);
            connection.Open();

            var res = await connection.QueryAsync<DetailedUserDto>(command);
            var dataItems = res as DetailedUserDto[] ?? res.ToArray();
            return dataItems.Length != 1
                ? Maybe<DetailedUserModel>.Nothing
                : Maybe.From(new DetailedUserModel(
                    dataItems.First().Id,
                    dataItems.First().EmailAddress,
                    dataItems.First().FirstName,
                    dataItems.First().LastName,
                    dataItems.First().IsLockable,
                    dataItems.First().WhenCreated,
                    dataItems.First().WhenLastAuthenticated,
                    dataItems.First().WhenLocked));
        }

        public async Task<Maybe<AuthenticationStatsModel>> GetAuthenticationStats(CancellationToken cancellationToken = default)
        {
            var command = new CommandDefinition(
                @"SELECT 
    (SELECT count(*) FROM [Identity].[User] where WhenCreated > DATEADD(DAY, -30, GETUTCDATE())) as TotalNewUsers
  , (SELECT count(*) FROM [Identity].[User] where WhenLastAuthenticated > DATEADD(DAY, -30, GETUTCDATE())) as TotalActiveUsers
  , (SELECT count(*) FROM [Identity].AuthenticationHistory where WhenHappened > DATEADD(DAY, -30, GETUTCDATE()) AND AuthenticationHistoryType = 1) as TotalLogins
  , (SELECT count(*) FROM [Identity].[User] where WhenLocked > DATEADD(DAY, -30, GETUTCDATE())) as TotalLockedAccounts",
                cancellationToken: cancellationToken);

            using var connection = new SqlConnection(this._querySettings.ConnectionString);
            connection.Open();

            var res = await connection.QueryAsync<AuthenticationStatsDto>(command);
            var dataItems = res as AuthenticationStatsDto[] ?? res.ToArray();
            return dataItems.Length != 1 ? Maybe<AuthenticationStatsModel>.Nothing : Maybe.From(new AuthenticationStatsModel(
                dataItems.First().TotalNewUsers,
                dataItems.First().TotalActiveUsers,
                dataItems.First().TotalLogins,
                dataItems.First().TotalLockedAccounts));
        }
    }
}