// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Microsoft.Extensions.Caching.Distributed;

namespace Initium.Portal.Core.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static async Task<Maybe<T>> TryGetValue<T>(this IDistributedCache memoryCache, object key, CancellationToken cancellationToken = default)
        {
            var keyHash = ConvertToString(key);
            IFormatter formatter = new BinaryFormatter();
            var bytes = await memoryCache.GetAsync(keyHash, cancellationToken);
            if (bytes == null)
            {
                return Maybe<T>.Nothing;
            }

            await using var memoryStream = new MemoryStream(bytes);
#pragma warning disable 618
            var obj = formatter.Deserialize(memoryStream);
#pragma warning restore 618
            if (!(obj is T realObj))
            {
                return Maybe<T>.Nothing;
            }

            return Maybe.From(realObj);
        }

        public static async Task AddValue<T>(this IDistributedCache memoryCache, object key, T value,
            DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
        {
            var keyHash = ConvertToString(key);
            IFormatter formatter = new BinaryFormatter();
            await using var stream = new MemoryStream();
#pragma warning disable 618
            formatter.Serialize(stream, value);
#pragma warning restore 618
            var bytes = stream.ToArray();
            await memoryCache.SetAsync(keyHash, bytes, options, cancellationToken);
        }

        private static string ConvertToString<T>(T value)
        {
            IFormatter formatter = new BinaryFormatter();
            using (var stream = new MemoryStream())
            {
                formatter.Serialize(stream, value);
                var bytes = stream.ToArray();
                return Encoding.UTF8.GetString(bytes);
            }
        }
    }
}