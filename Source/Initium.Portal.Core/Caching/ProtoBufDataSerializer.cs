// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using MaybeMonad;
using ProtoBuf;
using ResultMonad;

namespace Initium.Portal.Core.Caching
{
    public class ProtoBufDataSerializer : IDataSerializer
    {
        public Result<byte[]> SerializeToBytes<TEntity>(TEntity entity)
            where TEntity : class
        {
            if (entity == null)
            {
                return Result.Fail<byte[]>();
            }

            var bytes = Serialize(entity);
            return bytes.Length == 0 ? Result.Fail<byte[]>() : Result.Ok(bytes);
        }

        public Result<string> SerializeToBase64<TEntity>(TEntity entity)
            where TEntity : class
        {
            if (entity == null)
            {
                return Result.Fail<string>();
            }

            var bytes = Serialize(entity);
            return bytes.Length == 0 ? Result.Fail<string>() : Result.Ok(Convert.ToBase64String(bytes));
        }

        public Maybe<TEntity> DeserializeFromBytes<TEntity>(byte[] data)
            where TEntity : class
        {
            if (data == null || data.Length == 0)
            {
                return Maybe<TEntity>.Nothing;
            }

            var entity = Deserialize<TEntity>(data);
            return Maybe.From(entity);
        }

        public Maybe<TEntity> DeserializeFromBase64<TEntity>(string data)
            where TEntity : class
        {
            if (string.IsNullOrEmpty(data))
            {
                return Maybe<TEntity>.Nothing;
            }

            byte[] bytes;
            try
            {
                bytes = Convert.FromBase64String(data);
            }
            catch (FormatException)
            {
                return Maybe<TEntity>.Nothing;
            }

            var entity = Deserialize<TEntity>(bytes);
            return Maybe.From(entity);
        }

        private static byte[] Serialize<TEntity>(TEntity entity)
            where TEntity : class
        {
            using var stream = new MemoryStream();
            try
            {
                Serializer.Serialize(stream, entity);
            }
            catch (InvalidOperationException)
            {
                return Array.Empty<byte>();
            }

            var bytes = stream.ToArray();
            return bytes;
        }

        private static TEntity Deserialize<TEntity>(byte[] data)
            where TEntity : class
        {
            using var stream = new MemoryStream(data);
            TEntity entity;
            try
            {
                entity = Serializer.Deserialize<TEntity>(stream);
            }
            catch (InvalidOperationException)
            {
                return null;
            }

            return entity;
        }
    }
}