// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Stance.Core.Contracts.Domain;

namespace Stance.Domain.AggregatesModel.UserAggregate
{
    public interface IUserRepository : IRepository<IUser>
    {
        void Update(IUser user);

        Task<Maybe<IUser>> Find(Guid userId, CancellationToken cancellationToken = default);

        IUser Add(IUser user);

        Task<Maybe<IUser>> FindByEmailAddress(string emailAddress, CancellationToken cancellationToken = default);

        Task<Maybe<IUser>> FindByUserBySecurityToken(Guid tokenId, DateTime expiryDate, CancellationToken cancellationToken = default);
    }
}