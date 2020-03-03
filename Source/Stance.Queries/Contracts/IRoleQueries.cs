// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Stance.Queries.Models;
using Stance.Queries.Models.Role;

namespace Stance.Queries.Contracts
{
    public interface IRoleQueries
    {
        Task<StatusCheckModel> CheckForPresenceOfRoleByName(string name, CancellationToken cancellationToken = default);

        Task<StatusCheckModel> CheckForRoleUsageById(Guid roleId, CancellationToken cancellationToken = default);

        Task<Maybe<DetailedRoleModel>> GetDetailsOfRoleById(Guid roleId, CancellationToken cancellationToken = default);

        Task<Maybe<List<SimpleResourceModel>>> GetNestedSimpleResources(CancellationToken cancellationToken = default);

        Task<Maybe<List<SimpleRoleModel>>> GetSimpleRoles(CancellationToken cancellationToken = default);
    }
}