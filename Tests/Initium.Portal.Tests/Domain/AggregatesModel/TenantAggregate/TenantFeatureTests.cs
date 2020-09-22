// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Initium.Portal.Domain.AggregatesModel.TenantAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.AggregatesModel.TenantAggregate
{
    public class TenantFeatureTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var tenantFeature = new TenantFeature(TestVariables.FeatureId);
            Assert.Equal(TestVariables.FeatureId, tenantFeature.Id);

            foreach (var prop in tenantFeature.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(tenantFeature, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void Constructor_GivenPrivateIsCalled_ExpectObjectCreated()
        {
            var tenantFeature = (TenantFeature)Activator.CreateInstance(typeof(TenantFeature), true);
            Assert.NotNull(tenantFeature);

            foreach (var prop in tenantFeature.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(tenantFeature, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }
    }
}