// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;
using MaybeMonad;
using Microsoft.Extensions.Options;
using Stance.Queries.Contracts;
using Stance.Queries.Models;
using Stance.Queries.Models.Role;
using Stance.Queries.TransferObjects;
using Stance.Queries.TransferObjects.Role;

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

        public async Task<Maybe<DetailedRoleModel>> GetDetailsOfRoleById(Guid roleId, CancellationToken cancellationToken = default)
        {
            var parameters = new DynamicParameters();
            parameters.Add("roleId", roleId, DbType.Guid);

            var command = new CommandDefinition(
                @"SELECT r.Id, r.Name FROM [AccessProtection].[Role] r where r.id = @roleId;
                SELECT ResourceId FROM [AccessProtection].[RoleResource] where RoleId = @roleId;",
                parameters,
                cancellationToken: cancellationToken);

            using var connection = new SqlConnection(this._querySettings.ConnectionString);
            connection.Open();

            var res = await connection.QueryMultipleAsync(command);
            var detailedUseResult = await res.ReadAsync<DetailedRoleDto>();
            var dtos = detailedUseResult as DetailedRoleDto[] ?? detailedUseResult.ToArray();
            if (dtos.Length != 1)
            {
                return Maybe<DetailedRoleModel>.Nothing;
            }

            var entity = dtos.Single();

            var resourceResult = await res.ReadAsync<Guid>();

            return Maybe.From(
                new DetailedRoleModel(entity.Id, entity.Name, resourceResult.ToList()));
        }

        public async Task<Maybe<List<SimpleResourceModel>>> GetNestedSimpleResources(CancellationToken cancellationToken = default)
        {
            var command = new CommandDefinition(
                "SELECT Id, Name, ParentResourceId FROM [AccessProtection].[Resource]",
                cancellationToken: cancellationToken);

            using var connection = new SqlConnection(this._querySettings.ConnectionString);
            connection.Open();

            var res = await connection.QueryAsync<SimpleResourceDto>(command);
            var dtos = res as SimpleResourceDto[] ?? res.ToArray();
            if (dtos.Length == 0)
            {
                return Maybe<List<SimpleResourceModel>>.Nothing;
            }

            var items = dtos.Select(
                i => new SimpleResourceModel(i.Id, i.Name, i.ParentResourceId)).ToList();

            foreach (var i in items)
            {
                i.SetSimpleResources(items.Where(n => n.ParentId == i.Id).ToList());
            }

            return Maybe.From(new List<SimpleResourceModel>(items.Where(n => n.ParentId == Guid.Empty)));
        }

        public async Task<Maybe<List<SimpleRoleModel>>> GetSimpleRoles(CancellationToken cancellationToken = default)
        {
            var command = new CommandDefinition(
                "SELECT Id, Name FROM [AccessProtection].[Role]",
                cancellationToken: cancellationToken);

            using var connection = new SqlConnection(this._querySettings.ConnectionString);
            connection.Open();

            var res = await connection.QueryAsync<SimpleRoleDto>(command);
            var dtos = res as SimpleRoleDto[] ?? res.ToArray();
            if (dtos.Length < 1)
            {
                return Maybe<List<SimpleRoleModel>>.Nothing;
            }

            return Maybe.From(new List<SimpleRoleModel>(dtos.Select(x => new SimpleRoleModel(x.Id, x.Name))));
        }
    }
}