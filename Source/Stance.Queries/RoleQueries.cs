// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Data;
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
    public class RoleQueries : IRoleQueries
    {
        private readonly QuerySettings _querySettings;

        public RoleQueries(IOptions<QuerySettings> querySettings)
        {
            if (querySettings == null)
            {
                throw new ArgumentNullException(nameof(querySettings));
            }

            this._querySettings = querySettings.Value;
        }

        public async Task<StatusCheckModel> CheckForPresenceOfRoleByName(
            string name, CancellationToken cancellationToken = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("name", name, DbType.String);

            var command = new CommandDefinition(
                "select top 1 name from [AccessProtection].[Role] where name = @name",
                parameters,
                cancellationToken: cancellationToken);

            using var connection = new SqlConnection(this._querySettings.ConnectionString);
            connection.Open();

            var res = await connection.QueryAsync<PresenceCheckDto<string>>(command);
            var dtos = res as PresenceCheckDto<string>[] ?? res.ToArray();
            return new StatusCheckModel(dtos.Length > 0);
        }

        public Task<StatusCheckModel> CheckForRoleUsageById(
            Guid roleId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(new StatusCheckModel(false));
        }
    }
}