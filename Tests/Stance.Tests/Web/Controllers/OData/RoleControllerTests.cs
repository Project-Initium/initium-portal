// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Stance.Queries.Dynamic;
using Stance.Queries.Dynamic.Entities;
using Stance.Web.Controllers.OData.Role;
using Xunit;

namespace Stance.Tests.Web.Controllers.OData
{
    public class RoleControllerTests
    {
        public static IEnumerable<object[]> FilterData
        {
            get
            {
                return new[]
                {
                    new object[] { new RoleFilter { HasResources = true }, 2 },
                    new object[] { new RoleFilter { HasResources = true, HasNoResources = true }, 4 },
                    new object[] { new RoleFilter { HasNoResources = true }, 2 },
                    new object[] { new RoleFilter { HasUsers = true }, 1 },
                    new object[] { new RoleFilter { HasUsers = true, HasNoUsers = true }, 4 },
                    new object[] { new RoleFilter { HasNoUsers = true }, 3 },
                };
            }
        }

        [Theory]
        [MemberData(nameof(FilterData))]
        public void Filtered_GivenFilterIsNotNull_ExpectFilteredData(RoleFilter filter, int count)
        {
            var options = new DbContextOptionsBuilder<ODataContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ODataContext(options);
            context.Add(new Role
            {
                Id = Guid.NewGuid(),
                Name = "Role-1",
                ResourceCount = 0,
                UserCount = 3,
            });
            context.Add(new Role
            {
                Id = Guid.NewGuid(),
                Name = "Role-2",
                ResourceCount = 2,
                UserCount = 0,
            });
            context.Add(new Role
            {
                Id = Guid.NewGuid(),
                Name = "Role-3",
                ResourceCount = 4,
                UserCount = 0,
            });
            context.Add(new Role
            {
                Id = Guid.NewGuid(),
                Name = "Role-4",
                ResourceCount = 0,
                UserCount = 0,
            });
            context.SaveChanges();
            var roleController = new RoleController(context);
            var result = Assert.IsType<OkObjectResult>(roleController.Filtered(filter));
            var data = Assert.IsType<EntityQueryable<Role>>(result.Value);
            Assert.Equal(count, data.Count());
        }

        [Fact]
        public void Filtered_GivenFilterIsNull_ExpectUnfiltered()
        {
            var options = new DbContextOptionsBuilder<ODataContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ODataContext(options);
            context.Add(new Role
            {
                Id = Guid.NewGuid(),
                Name = "Role-1",
                ResourceCount = 0,
                UserCount = 3,
            });
            context.Add(new Role
            {
                Id = Guid.NewGuid(),
                Name = "Role-2",
                ResourceCount = 2,
                UserCount = 0,
            });
            context.Add(new Role
            {
                Id = Guid.NewGuid(),
                Name = "Role-3",
                ResourceCount = 4,
                UserCount = 4,
            });
            context.Add(new Role
            {
                Id = Guid.NewGuid(),
                Name = "Role-4",
                ResourceCount = 0,
                UserCount = 0,
            });
            context.SaveChanges();

            var roleController = new RoleController(context);
            var result = Assert.IsType<OkObjectResult>(roleController.Filtered(null));
            var data = Assert.IsType<InternalDbSet<Role>>(result.Value);
            Assert.Equal(4, data.AsQueryable().Count());
        }
    }
}