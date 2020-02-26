// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Stance.Queries.Models;
using Stance.Queries.Models.User;

namespace Stance.Queries.Contracts
{
    public interface IUserQueries
    {
        Task<StatusCheckModel> CheckForPresenceOfAnyUser(CancellationToken cancellationToken = default);

        Task<Maybe<ProfileModel>> GetProfileForCurrentUser(CancellationToken cancellationToken = default);

        Task<StatusCheckModel> CheckForPresenceOfUserByEmailAddress(string emailAddress, CancellationToken cancellationToken = default);

        Task<Maybe<DetailedUserModel>> GetDetailsOfUserById(Guid userId, CancellationToken cancellationToken = default);

        Task<Maybe<AuthenticationStatsModel>> GetAuthenticationStats(CancellationToken cancellationToken = default);
    }
}