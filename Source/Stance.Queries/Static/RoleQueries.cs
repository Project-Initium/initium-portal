// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MaybeMonad;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static.Models;
using Stance.Queries.Static.Models.Role;
using Stance.Queries.Static.TransferObjects;
using Stance.Queries.Static.TransferObjects.Role;

namespace Stance.Queries.Static
{
    public class RoleQueries : IRoleQueries
    {
        private readonly IConnectionProvider _connectionProvider;

        public RoleQueries(IConnectionProvider connectionProvider)
        {
            this._connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
        }

        public async Task<StatusCheckModel> CheckForPresenceOfRoleByName(string name)
        {
            var parameters = new DynamicParameters();
            parameters.Add("name", name, DbType.String);

            var command = new CommandDefinition(
                "[Portal].[uspCheckForPresenceOfRoleByName]",
                parameters,
                commandType: CommandType.StoredProcedure);

            using var connection = this._connectionProvider.GetConnection();
            connection.Open();

            var res = await connection.QueryAsync<PresenceCheckDto<Guid>>(command);
            var dtos = res as PresenceCheckDto<Guid>[] ?? res.ToArray();
            return new StatusCheckModel(dtos.Length > 0);
        }

        public async Task<StatusCheckModel> CheckForRoleUsageById(Guid roleId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("roleId", roleId, DbType.String);

            var command = new CommandDefinition(
                "[Portal].[uspCheckForRoleUsageById]",
                parameters,
                commandType: CommandType.StoredProcedure);

            using var connection = this._connectionProvider.GetConnection();
            connection.Open();

            var res = await connection.QueryAsync<PresenceCheckDto<Guid>>(command);
            var dtos = res as PresenceCheckDto<Guid>[] ?? res.ToArray();
            return new StatusCheckModel(dtos.Length > 0);
        }

        public async Task<Maybe<DetailedRoleModel>> GetDetailsOfRoleById(Guid roleId)
        {
            var parameters = new DynamicParameters();
            parameters.Add("roleId", roleId, DbType.Guid);

            var command = new CommandDefinition(
                "[Portal].[uspGetDetailsOfRoleById]",
                parameters,
                commandType: CommandType.StoredProcedure);

            using var connection = this._connectionProvider.GetConnection();
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

        public async Task<Maybe<List<SimpleResourceModel>>> GetNestedSimpleResources()
        {
            var command = new CommandDefinition(
                "[Portal].[uspGetNestedSimpleResources]",
                commandType: CommandType.StoredProcedure);

            using var connection = this._connectionProvider.GetConnection();
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

        public async Task<Maybe<List<SimpleRoleModel>>> GetSimpleRoles()
        {
            var command = new CommandDefinition(
                "[Portal].[uspGetSimpleRoles]",
                commandType: CommandType.StoredProcedure);

            using var connection = this._connectionProvider.GetConnection();
            connection.Open();

            var res = await connection.QueryAsync<SimpleRoleDto>(command);
            var dtos = res as SimpleRoleDto[] ?? res.ToArray();
            if (dtos.Length == 0)
            {
                return Maybe<List<SimpleRoleModel>>.Nothing;
            }

            return Maybe.From(new List<SimpleRoleModel>(dtos.Select(x => new SimpleRoleModel(x.Id, x.Name))));
        }
    }
}