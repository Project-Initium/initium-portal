﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Initium.Portal.Core.Database;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Entities;
using Initium.Portal.Queries.Models;
using Initium.Portal.Queries.Models.Role;
using MaybeMonad;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries
{
    public class RoleQueryService : IRoleQueryService
    {
        private readonly GenericDataContext _context;

        public RoleQueryService(GenericDataContext context)
        {
            this._context = context;
        }

        public IQueryable<RoleReadEntity> QueryableEntity => this._context.Set<RoleReadEntity>();

        public async Task<Maybe<DetailedRoleModel>> GetDetailsOfRoleById(Guid roleId)
        {
            var data = await this.QueryableEntity
                .Include(x => x.Resources)
                .Where(role => role.Id == roleId)
                .Select(role => new
                {
                    role.Id,
                    role.Name,
                    Resources = role.Resources.Select(resource => resource.Id),
                })
                .SingleOrDefaultAsync();

            return data == null ? Maybe<DetailedRoleModel>.Nothing : Maybe.From(
                new DetailedRoleModel(data.Id, data.Name, data.Resources.ToList()));
        }

        public async Task<Maybe<List<SimpleRoleModel>>> GetSimpleRoles()
        {
            var data = await this.QueryableEntity.Select(x => new { x.Id, x.Name }).ToListAsync();
            return !data.Any() ? Maybe<List<SimpleRoleModel>>.Nothing : Maybe.From(new List<SimpleRoleModel>(data.Select(x => new SimpleRoleModel(x.Id, x.Name))));
        }
    }
}