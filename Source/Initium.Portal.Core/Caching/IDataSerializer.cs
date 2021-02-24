// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using MaybeMonad;
using ResultMonad;

namespace Initium.Portal.Core.Caching
{
    public interface IDataSerializer
    {
        Result<byte[]> SerializeToBytes<TEntity>(TEntity entity)
            where TEntity : class;

        Result<string> SerializeToBase64<TEntity>(TEntity entity)
            where TEntity : class;

        Maybe<TEntity> DeserializeFromBytes<TEntity>(byte[] data)
            where TEntity : class;

        Maybe<TEntity> DeserializeFromBase64<TEntity>(string data)
            where TEntity : class;
    }
}