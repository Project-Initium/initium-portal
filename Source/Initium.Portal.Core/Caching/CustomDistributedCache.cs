// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Extensions;
using MaybeMonad;
using Microsoft.Extensions.Caching.Distributed;

namespace Initium.Portal.Core.Caching
{
    public class CustomDistributedCache : ICustomDistributedCache
    {
        private readonly IDataSerializer _dataSerializer;
        private readonly IDistributedCache _distributedCache;

        public CustomDistributedCache(IDataSerializer dataSerializer, IDistributedCache distributedCache)
        {
            this._dataSerializer = dataSerializer;
            this._distributedCache = distributedCache;
        }

        public async Task<Maybe<TEntity>> TryGetValue<TEntity>(string key, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var bytes = await this._distributedCache.GetAsync(key, cancellationToken);

            if (bytes == null || bytes == Array.Empty<byte>())
            {
                return Maybe<TEntity>.Nothing;
            }

            return this._dataSerializer.DeserializeFromBytes<TEntity>(bytes);
        }

        public async Task AddValue<TEntity>(string key, TEntity value,
            DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            var result = this._dataSerializer.SerializeToBytes(value);
            if (result.IsSuccess)
            {
                await this._distributedCache.SetAsync(key, result.Value, options, cancellationToken);
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await this._distributedCache.RemoveAsync(key, cancellationToken);
        }
    }
}