// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Initium.Portal.Queries.Dynamic;
using Initium.Portal.Queries.Dynamic.Entities;
using Initium.Portal.Web.Controllers.OData.User;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Xunit;

namespace Initium.Portal.Tests.Web.Controllers.OData
{
    public class UserControllerTests
    {
        public static IEnumerable<object[]> FilterData
        {
            get
            {
                return new[]
                {
                    new object[] { new UserFilter { Verified = true }, 3 },
                    new object[] { new UserFilter { Verified = true, Unverified = true }, 6 },
                    new object[] { new UserFilter { Unverified = true }, 3 },
                    new object[] { new UserFilter { Locked = true }, 4 },
                    new object[] { new UserFilter { Locked = true, Unlocked = true }, 6 },
                    new object[] { new UserFilter { Unlocked = true }, 2 },
                    new object[] { new UserFilter { Admin = true }, 3 },
                    new object[] { new UserFilter { Admin = true, NonAdmin = true }, 6 },
                    new object[] { new UserFilter { NonAdmin = true }, 3 },
                };
            }
        }

        [Fact]
        public void Filtered_GivenFilterIsNull_ExpectUnfiltered()
        {
            var options = new DbContextOptionsBuilder<ODataContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ODataContext(options);
            context.Add(new User
            {
                Id = Guid.NewGuid(),
                IsAdmin = false,
                IsLocked = true,
                IsVerified = true,
            });
            context.Add(new User
            {
                Id = Guid.NewGuid(),
                IsAdmin = true,
                IsLocked = false,
                IsVerified = true,
            });
            context.Add(new User
            {
                Id = Guid.NewGuid(),
                IsAdmin = true,
                IsLocked = true,
                IsVerified = false,
            });
            context.Add(new User
            {
                Id = Guid.NewGuid(),
                IsAdmin = false,
                IsLocked = true,
                IsVerified = false,
            });
            context.Add(new User
            {
                Id = Guid.NewGuid(),
                IsAdmin = true,
                IsLocked = true,
                IsVerified = true,
            });
            context.Add(new User
            {
                Id = Guid.NewGuid(),
                IsAdmin = false,
                IsLocked = false,
                IsVerified = false,
            });
            context.SaveChanges();
            var userController = new UserController(context);
            var result = Assert.IsType<OkObjectResult>(userController.Filtered(null));
            var data = Assert.IsType<InternalDbSet<User>>(result.Value);
            Assert.Equal(6, data.AsQueryable().Count());
        }

        [Theory]
        [MemberData(nameof(FilterData))]
        public void Filtered_GivenFilterIsNotNull_ExpectFilteredData(UserFilter filter, int count)
        {
            var options = new DbContextOptionsBuilder<ODataContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ODataContext(options);
            context.Add(new User
            {
                Id = Guid.NewGuid(),
                IsAdmin = false,
                IsLocked = true,
                IsVerified = true,
            });
            context.Add(new User
            {
                Id = Guid.NewGuid(),
                IsAdmin = true,
                IsLocked = false,
                IsVerified = true,
            });
            context.Add(new User
            {
                Id = Guid.NewGuid(),
                IsAdmin = true,
                IsLocked = true,
                IsVerified = false,
            });
            context.Add(new User
            {
                Id = Guid.NewGuid(),
                IsAdmin = false,
                IsLocked = true,
                IsVerified = false,
            });
            context.Add(new User
            {
                Id = Guid.NewGuid(),
                IsAdmin = true,
                IsLocked = true,
                IsVerified = true,
            });
            context.Add(new User
            {
                Id = Guid.NewGuid(),
                IsAdmin = false,
                IsLocked = false,
                IsVerified = false,
            });
            context.SaveChanges();
            var userController = new UserController(context);
            var result = Assert.IsType<OkObjectResult>(userController.Filtered(filter));
            var data = Assert.IsType<EntityQueryable<User>>(result.Value);
            Assert.Equal(count, data.AsQueryable().Count());
        }
    }
}