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
            parameters.Add("emailAddress", emailAddress, DbType.Guid);

            var command = new CommandDefinition(
                "select top 1 id from [identity].[user] where emailAddress = @emailAddress",
                cancellationToken: cancellationToken);

            using var connection = new SqlConnection(this._querySettings.ConnectionString);
            connection.Open();

            var res = await connection.QueryAsync<PresenceCheckDto<Guid>>(command);
            var dataItems = res as PresenceCheckDto<Guid>[] ?? res.ToArray();
            return new StatusCheckModel(dataItems.Length > 0);
        }
    }
}