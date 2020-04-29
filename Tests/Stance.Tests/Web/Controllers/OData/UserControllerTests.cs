// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Stance.Queries.Dynamic;
using Stance.Queries.Dynamic.Entities;
using Stance.Web.Controllers.OData.User;
using Xunit;

namespace Stance.Tests.Web.Controllers.OData
{
    public class UserControllerTests
    {
        [Fact]
        public void Get_GivenValidRequest_ExpectUnfiltered()
        {
            var options = new DbContextOptionsBuilder<ODataContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ODataContext(options);
            context.Add(new User());
            context.Add(new User());
            context.Add(new User());
            context.Add(new User());
            context.SaveChanges();
            var userController = new UserController(context);
            var result = userController.Get();
            Assert.Equal(4, result.ToList().Count);
        }
    }
}