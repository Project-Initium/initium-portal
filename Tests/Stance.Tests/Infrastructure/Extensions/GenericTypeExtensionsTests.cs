// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Stance.Infrastructure.Extensions;
using Xunit;

namespace Stance.Tests.Infrastructure.Extensions
{
    public class GenericTypeExtensionsTests
    {
        [Fact]
        public void GetGenericTypeName_GivenObjectIsNotGeneric_ExpectTypeName()
        {
            var @string = "some-string";
            var typeName = @string.GetGenericTypeName();
            Assert.Equal("String", typeName);
        }

        [Fact]
        public void GetGenericTypeName_GivenObjectIsGeneric_ExpectTypeNameWithArgument()
        {
            var listing = new List<string>();
            var typeName = listing.GetGenericTypeName();
            Assert.Equal("List<String>", typeName);
        }
    }
}