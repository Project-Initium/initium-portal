// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Caching;
using ProtoBuf;
using Xunit;

namespace Initium.Portal.Tests.Core.Caching
{
    public class ProtoBufDataSerializerTests
    {
        private const string DataAsString = "CHsSBGJ1eno=";

        private static readonly byte[] DataAsByte =
        {
            8, 123, 18, 4, 98, 117, 122, 122,
        };

        [Fact]
        public void SerializeToBytes_GivenEntityIsNull_ExpectFailure()
        {
            var serializer = new ProtoBufDataSerializer();
            var result = serializer.SerializeToBytes<DummyClass>(null);
            Assert.True(result.IsFailure);
        }

        [Fact]
        public void SerializeToBytes_GivenEntityIsNotDecorated_ExpectFailure()
        {
            var serializer = new ProtoBufDataSerializer();
            var result = serializer.SerializeToBytes(new DummyClass());
            Assert.True(result.IsFailure);
        }

        [Fact]
        public void SerializeToBytes_ExpectResultWithData()
        {
            var serializer = new ProtoBufDataSerializer();
            var result = serializer.SerializeToBytes(new DecoratedDummyClass(123, "buzz"));
            Assert.True(result.IsSuccess);
            Assert.Equal(DataAsByte, result.Value);
        }

        [Fact]
        public void SerializeToBase64_GivenEntityIsNull_ExpectFailure()
        {
            var serializer = new ProtoBufDataSerializer();
            var result = serializer.SerializeToBase64<DummyClass>(null);
            Assert.True(result.IsFailure);
        }

        [Fact]
        public void SerializeToBase64_GivenEntityIsNotDecorated_ExpectFailure()
        {
            var serializer = new ProtoBufDataSerializer();
            var result = serializer.SerializeToBase64(new DummyClass());
            Assert.True(result.IsFailure);
        }

        [Fact]
        public void SerializeToBase64_ExpectResultWithData()
        {
            var serializer = new ProtoBufDataSerializer();
            var result = serializer.SerializeToBase64(new DecoratedDummyClass(123, "buzz"));
            Assert.True(result.IsSuccess);
            Assert.Equal(DataAsString, result.Value);
        }

        [Fact]
        public void DeserializeFromBytes_GivenDataIsNull_ExpectMaybeWithNoData()
        {
            var serializer = new ProtoBufDataSerializer();
            var maybe = serializer.DeserializeFromBytes<DummyClass>(null);
            Assert.True(maybe.HasNoValue);
        }

        [Fact]
        public void DeserializeFromBytes_GivenDataIsEmpty_ExpectMaybeWithNoData()
        {
            var serializer = new ProtoBufDataSerializer();
            var maybe = serializer.DeserializeFromBytes<DummyClass>(Array.Empty<byte>());
            Assert.True(maybe.HasNoValue);
        }

        [Fact]
        public void DeserializeFromBytes_GivenEntityIsNotDecorated_ExpectMaybeWithNoData()
        {
            var serializer = new ProtoBufDataSerializer();
            var maybe = serializer.DeserializeFromBytes<DummyClass>(DataAsByte);
            Assert.True(maybe.HasNoValue);
        }

        [Fact]
        public void DeserializeFromBytes_ExpectMaybeWithData()
        {
            var serializer = new ProtoBufDataSerializer();
            var maybe = serializer.DeserializeFromBytes<DecoratedDummyClass>(DataAsByte);
            Assert.True(maybe.HasValue);
            Assert.Equal(123, maybe.Value.Foo);
            Assert.Equal("buzz", maybe.Value.Fiz);
        }

        [Fact]
        public void DeserializeFromBase64_GivenDataIsNull_ExpectMaybeWithNoData()
        {
            var serializer = new ProtoBufDataSerializer();
            var maybe = serializer.DeserializeFromBase64<DummyClass>(null);
            Assert.True(maybe.HasNoValue);
        }

        [Fact]
        public void DeserializeFromBase64_GivenDataIsEmptyString_ExpectMaybeWithNoData()
        {
            var serializer = new ProtoBufDataSerializer();
            var maybe = serializer.DeserializeFromBase64<DummyClass>(string.Empty);
            Assert.True(maybe.HasNoValue);
        }

        [Fact]
        public void DeserializeFromBase64_GivenDataIsNotBase64_ExpectMaybeWithNoData()
        {
            var serializer = new ProtoBufDataSerializer();
            var maybe = serializer.DeserializeFromBase64<DummyClass>("not-base64");
            Assert.True(maybe.HasNoValue);
        }

        [Fact]
        public void DeserializeFromBase64_GivenEntityIsNotDecorated_ExpectMaybeWithNoData()
        {
            var serializer = new ProtoBufDataSerializer();
            var maybe = serializer.DeserializeFromBase64<DummyClass>(DataAsString);
            Assert.True(maybe.HasNoValue);
        }

        [Fact]
        public void DeserializeFromBase64_ExpectMaybeWithData()
        {
            var serializer = new ProtoBufDataSerializer();
            var maybe = serializer.DeserializeFromBase64<DecoratedDummyClass>(DataAsString);
            Assert.True(maybe.HasValue);
            Assert.Equal(123, maybe.Value.Foo);
            Assert.Equal("buzz", maybe.Value.Fiz);
        }

        public class DummyClass
        {
        }

        [ProtoContract(SkipConstructor = true, UseProtoMembersOnly = true)]
        public class DecoratedDummyClass
        {
            public DecoratedDummyClass(int foo, string fiz)
            {
                this.Foo = foo;
                this.Fiz = fiz;
            }

            [ProtoMember(1)]
            public int Foo { get; private set; }

            [ProtoMember(2)]
            public string Fiz { get; private set; }
        }
    }
}