// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.AggregatesModel.RoleAggregate
{
    public class RoleTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_ExpectPropertiesAreSet()
        {
            var role = new Role(TestVariables.RoleId, "name", new List<Guid>
            {
                TestVariables.ResourceId,
            });

            Assert.Equal(TestVariables.RoleId, role.Id);
            Assert.Equal("name", role.Name);
            var roleResource = Assert.Single(role.RoleResources);
            Assert.NotNull(roleResource);

            foreach (var prop in role.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(role, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void Constructor_GivenPrivateIsCalled_ExpectObjectCreated()
        {
            var role = (Role)Activator.CreateInstance(typeof(Role), true);
            Assert.NotNull(role);

            foreach (var prop in role.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(role, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void SetResources_GiveValidArguments_ExpectResourcesToBeUpdate()
        {
            var permResource = Guid.NewGuid();
            var tempResource = Guid.NewGuid();
            var newResource = Guid.NewGuid();
            var dupeResource = permResource;

            var role = new Role(Guid.NewGuid(), string.Empty, new List<Guid>
            {
                permResource,
                tempResource,
            });

            role.SetResources(new List<Guid>
            {
                permResource,
                newResource,
                dupeResource,
            });

            Assert.Single(role.RoleResources, x => x.Id == permResource);
            Assert.Single(role.RoleResources, x => x.Id == newResource);
            Assert.DoesNotContain(role.RoleResources, resource => resource.Id == tempResource);
        }

        [Fact]
        public void UpdateName_GiveValidArguments_ExpectNameIsUpdated()
        {
            var role = new Role(TestVariables.RoleId, string.Empty, new List<Guid>());
            role.UpdateName("new-name");

            Assert.Equal("new-name", role.Name);
        }
    }
}