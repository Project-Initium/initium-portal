// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Database;
using ResultMonad;

namespace Initium.Portal.Core.Contracts.Domain
{
    public interface IUnitOfWork : IDisposable
    {
        Task<ResultWithError<IPersistenceError>> SaveEntitiesAsync(CancellationToken cancellationToken = default);
    }
}