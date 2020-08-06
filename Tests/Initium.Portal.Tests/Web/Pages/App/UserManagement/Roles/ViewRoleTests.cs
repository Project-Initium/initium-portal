// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models.Role;
using Initium.Portal.Web.Pages.App.UserManagement.Roles;
using MaybeMonad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.App.UserManagement.Roles
{
    public class ViewRoleTests
    {
        [Fact]
        public async Task OnGetAsync_GivenRoleIsNotInSystem_ExpectNotFoundResultReturned()
        {
            var roleQueries = new Mock<IRoleQueryService>();
            roleQueries.Setup(x => x.GetDetailsOfRoleById(It.IsAny<Guid>()))
                .ReturnsAsync(() => Maybe<DetailedRoleModel>.Nothing);

            var page = new ViewRole(roleQueries.Object);

            var result = await page.OnGetAsync();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_GivenRoleIsInSystem_ExpectDataToBeSetAndPageResultReturned()
        {
            var roleQueries = new Mock<IRoleQueryService>();
            roleQueries.Setup(x => x.GetDetailsOfRoleById(It.IsAny<Guid>()))
                .ReturnsAsync(() => Maybe.From(new DetailedRoleModel(
                    TestVariables.RoleId,
                    "name",
                    new List<Guid> { TestVariables.ResourceId })));

            var page = new ViewRole(roleQueries.Object);

            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.NotNull(page.Role);
            Assert.Equal("name", page.Name);
        }
    }
}