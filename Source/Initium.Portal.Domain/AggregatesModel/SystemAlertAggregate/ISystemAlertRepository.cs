// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Domain;
using MaybeMonad;

namespace Initium.Portal.Domain.AggregatesModel.SystemAlertAggregate
{
    public interface ISystemAlertRepository : IRepository<ISystemAlert>
    {
        ISystemAlert Add(ISystemAlert systemAlert);

        Task<Maybe<ISystemAlert>> Find(Guid id, CancellationToken cancellationToken = default);

        void Delete(ISystemAlert systemAlert);

        void Update(ISystemAlert systemAlert);
    }
}