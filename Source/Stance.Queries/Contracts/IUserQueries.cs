// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Stance.Queries.Models;

namespace Stance.Queries.Contracts
{
    public interface IUserQueries
    {
        Task<StatusCheckModel> CheckForPresenceOfAnyUser(CancellationToken cancellationToken = default);
    }
}