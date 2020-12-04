// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Extensions;
using Initium.Portal.Web.Infrastructure.PageModels;
using Xunit;

namespace Initium.Portal.Tests.Core.Exceptions
{
    public class EnumExtensionsTests
    {
        [Fact]
        public void GetDisplay_GivenNameForValueIsNotFound_ExpectEmptyString()
        {
            var systemFeature = (SystemFeatures)100;
            Assert.Equal(string.Empty, systemFeature.GetDisplay());
        }

        [Fact]
        public void GetDisplay_GivenCustomAttributeIsNotFound_ExpectEmptyString()
        {
            var systemFeature = PrgState.Success;
            Assert.Equal(string.Empty, systemFeature.GetDisplay());
        }

        [Fact]
        public void GetDisplay_GivenCustomAttributeIsFound_ExpectNameReturn()
        {
            var systemFeature = SystemFeatures.MfaApp;
            Assert.Equal("MFA App", systemFeature.GetDisplay());
        }
    }
}