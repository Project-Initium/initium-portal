// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Extensions;
using Microsoft.Extensions.Caching.Distributed;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Core.Extensions
{
    public class DistributedCacheExtensionsTests
    {
        [Fact]
        public async Task TryGetValue_GivenNothingFoundForKey_ExpectMaybeWithNoData()
        {
            var memoryCache = new Mock<IDistributedCache>();
            memoryCache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(null as byte[]);
            var result = await memoryCache.Object.TryGetValue<Dummy>("some-key", CancellationToken.None);

            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task TryGetValue_GivenObjectWasFoundButNotTheRightType_ExpectMaybeWithNoData()
        {
            var memoryCache = new Mock<IDistributedCache>();
            memoryCache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(SerializeObject(new AnotherDummy("data")));

            var result = await memoryCache.Object.TryGetValue<Dummy>("some-key", CancellationToken.None);

            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task TryGetValue_GivenObjectWasFoundOfCorrectType_ExpectMaybeWithData()
        {
            var memoryCache = new Mock<IDistributedCache>();
            memoryCache.Setup(x => x.GetAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(SerializeObject(new Dummy("some-data")));

            var result = await memoryCache.Object.TryGetValue<Dummy>("some-key", CancellationToken.None);

            Assert.True(result.HasValue);
            Assert.Equal("some-data", result.Value.Foo);
        }

        [Fact]
        public async Task AddValue_GivenCorrectArguments_ExpectDataToBeCached()
        {
            var data = SerializeObject(new Dummy("some-data"));
            var memoryCache = new Mock<IDistributedCache>();
            memoryCache.Setup(x => x.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.Is<DistributedCacheEntryOptions>(y => y.AbsoluteExpiration == TestVariables.Now.AddSeconds(120)),
                It.IsAny<CancellationToken>())).Verifiable();

            await memoryCache.Object.AddValue("some-key", data, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = TestVariables.Now.AddSeconds(120),
            }, CancellationToken.None);
            memoryCache.Verify();
        }

        private static byte[] SerializeObject<T>(T value)
        {
            var formatter = new BinaryFormatter();
            using var stream = new MemoryStream();
#pragma warning disable 618
            formatter.Serialize(stream, value);
#pragma warning restore 618
            return stream.ToArray();
        }

        [Serializable]
        public class Dummy
        {
            public Dummy(string data)
            {
                this.Foo = data;
            }

            public string Foo { get; }
        }

        [Serializable]
        public class AnotherDummy
        {
            public AnotherDummy(string data)
            {
                this.Bar = data;
            }

            public string Bar { get; }
        }
    }
}