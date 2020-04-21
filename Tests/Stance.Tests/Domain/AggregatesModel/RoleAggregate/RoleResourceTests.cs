// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Stance.Domain.AggregatesModel.RoleAggregate;
using Xunit;

namespace Stance.Tests.Domain.AggregatesModel.RoleAggregate
{
    public class RoleResourceTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var roleResource = new RoleResource(TestVariables.ResourceId);
            Assert.Equal(TestVariables.ResourceId, roleResource.Id);

            foreach (var prop in roleResource.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(roleResource, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void Constructor_GivenPrivateIsCalled_ExpectObjectCreated()
        {
            var roleResource = (RoleResource)Activator.CreateInstance(typeof(RoleResource), true);
            Assert.NotNull(roleResource);

            foreach (var prop in roleResource.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(roleResource, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }
    }
}