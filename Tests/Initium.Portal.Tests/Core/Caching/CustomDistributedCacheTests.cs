// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Caching;
using MaybeMonad;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Core.Caching
{
    public class CustomDistributedCacheTests
    {
        [Fact]
        public async Task TryGetValue_GivenCacheReturnsNull_ExpectMaybeWithNoData()
        {
            var dataSerializer = new Mock<IDataSerializer>();

            var memoryCache = new Mock<IDistributedCache>();
            memoryCache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as byte[]);

            var customDistributedCache = new CustomDistributedCache(dataSerializer.Object, memoryCache.Object);
            var result = await customDistributedCache.TryGetValue<Dummy>("some-key", CancellationToken.None);

            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task TryGetValue_GivenCacheReturnsEmptyArray_ExpectMaybeWithNoData()
        {
            var dataSerializer = new Mock<IDataSerializer>();

            var memoryCache = new Mock<IDistributedCache>();
            memoryCache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Array.Empty<byte>());

            var customDistributedCache = new CustomDistributedCache(dataSerializer.Object, memoryCache.Object);
            var result = await customDistributedCache.TryGetValue<Dummy>("some-key", CancellationToken.None);

            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task TryGetValue_ExpectMaybeWithData()
        {
            var dataSerializer = new Mock<IDataSerializer>();
            dataSerializer.Setup(x => x.DeserializeFromBytes<Dummy>(It.IsAny<byte[]>()))
                .Returns(Maybe.From(new Dummy("some-data")));

            var memoryCache = new Mock<IDistributedCache>();
            memoryCache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new byte[] { 1 });

            var customDistributedCache = new CustomDistributedCache(dataSerializer.Object, memoryCache.Object);
            var result = await customDistributedCache.TryGetValue<Dummy>("some-key", CancellationToken.None);

            Assert.True(result.HasValue);
            Assert.Equal("some-data", result.Value.Foo);
        }

        [Fact]
        public async Task AddValue_GivenDataFailsToSerialize_ExpectNoDataToBeCached()
        {
            var dataSerializer = new Mock<IDataSerializer>();
            dataSerializer.Setup(x => x.SerializeToBytes(It.IsAny<Dummy>()))
                .Returns(Result.Fail<byte[]>());
            var memoryCache = new Mock<IDistributedCache>();

            var customDistributedCache = new CustomDistributedCache(dataSerializer.Object, memoryCache.Object);
            await customDistributedCache.AddValue("key", new Dummy("some-data"), new DistributedCacheEntryOptions());

            memoryCache.Verify(
                x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task AddValue_ExpectDataToBeCached()
        {
            var dataSerializer = new Mock<IDataSerializer>();
            dataSerializer.Setup(x => x.SerializeToBytes(It.IsAny<Dummy>()))
                .Returns(Result.Ok(Array.Empty<byte>()));
            var memoryCache = new Mock<IDistributedCache>();

            var customDistributedCache = new CustomDistributedCache(dataSerializer.Object, memoryCache.Object);
            await customDistributedCache.AddValue("key", new Dummy("some-data"), new DistributedCacheEntryOptions());

            memoryCache.Verify(
                x => x.SetAsync(It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<DistributedCacheEntryOptions>(),
                    It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_ExpectDataToBeRemoved()
        {
            var dataSerializer = new Mock<IDataSerializer>();
            var memoryCache = new Mock<IDistributedCache>();
            memoryCache.Setup(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()));

            var customDistributedCache = new CustomDistributedCache(dataSerializer.Object, memoryCache.Object);
            await customDistributedCache.RemoveAsync("key");

            memoryCache.Verify(x => x.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        public class Dummy
        {
            public Dummy(string data)
            {
                this.Foo = data;
            }

            public string Foo { get; }
        }
    }
}