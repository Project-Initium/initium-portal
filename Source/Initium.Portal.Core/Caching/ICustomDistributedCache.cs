// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Microsoft.Extensions.Caching.Distributed;

namespace Initium.Portal.Core.Caching
{
    public interface ICustomDistributedCache
    {
        Task<Maybe<TEntity>> TryGetValue<TEntity>(string key, CancellationToken cancellationToken = default)
            where TEntity : class;

        Task AddValue<TEntity>(string key, TEntity value,
            DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
            where TEntity : class;

        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    }
}