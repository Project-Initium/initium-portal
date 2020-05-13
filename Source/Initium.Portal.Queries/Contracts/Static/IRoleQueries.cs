// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Initium.Portal.Queries.Static.Models;
using Initium.Portal.Queries.Static.Models.Role;
using MaybeMonad;

namespace Initium.Portal.Queries.Contracts.Static
{
    public interface IRoleQueries
    {
        Task<StatusCheckModel> CheckForPresenceOfRoleByName(string name);

        Task<StatusCheckModel> CheckForRoleUsageById(Guid roleId);

        Task<Maybe<DetailedRoleModel>> GetDetailsOfRoleById(Guid roleId);

        Task<Maybe<List<SimpleResourceModel>>> GetNestedSimpleResources();

        Task<Maybe<List<SimpleRoleModel>>> GetSimpleRoles();
    }
}