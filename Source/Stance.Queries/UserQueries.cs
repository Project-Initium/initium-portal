// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Options;
using Stance.Queries.Contracts;
using Stance.Queries.Models;
using Stance.Queries.TransferObjects;

namespace Stance.Queries
{
    public sealed class UserQueries : IUserQueries
    {
        private readonly QuerySettings _querySettings;

        public UserQueries(IOptions<QuerySettings> querySettings)
        {
            if (querySettings == null)
            {
                throw new ArgumentNullException(nameof(querySettings));
            }

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
    }
}