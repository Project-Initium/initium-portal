// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Stance.Domain.AggregatesModel.RoleAggregate;
using Xunit;

namespace Stance.Tests.Domain.AggregatesModel.RoleAggregate
{
    public class RoleTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_ExpectPropertiesAreSet()
        {
            var id = Guid.NewGuid();
            var name = new string('*', 5);
            var resourceId = Guid.NewGuid();

            var role = new Role(id, name, new List<Guid>
            {
                resourceId,
            });

            Assert.Equal(id, role.Id);
            Assert.Equal(name, role.Name);
            Assert.Contains(role.RoleResources, resource => resource.Id == resourceId);
        }

        [Fact]
        public void SetResources_GiveValidArguments_ExpectResourcesByUpdate()
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
            var id = Guid.NewGuid();
            var newName = new string('*', 5);

            var role = new Role(id, string.Empty, new List<Guid>());
            role.UpdateName(newName);

            Assert.Equal(newName, role.Name);
        }
    }
}