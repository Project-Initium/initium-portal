// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Stance.Queries.Models;

namespace Stance.Queries.Contracts
{
    public interface IRoleQueries
    {
        Task<StatusCheckModel> CheckForPresenceOfRoleByName(string name, CancellationToken cancellationToken = default);

        Task<StatusCheckModel> CheckForRoleUsageById(Guid roleId, CancellationToken cancellationToken = default);
    }
}