// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Initium.Portal.Core;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Queries.Contracts.Static;
using Initium.Portal.Queries.Static.Models.User;
using Initium.Portal.Web.Pages.App.UserManagement.Users;
using MaybeMonad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.App.UserManagement.Users
{
    public class ViewUserTests
    {
        [Fact]
        public async Task OnGetAsync_GivenUserDoesNotExist_ExpectNotFoundResultReturn()
        {
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe<DetailedUserModel>.Nothing);
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var page = new ViewUser(userQueries.Object, currentAuthenticatedUserProvider.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_GivenNoAuthenticateUser_ExpectNotFoundResultReturn()
        {
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe<DetailedUserModel>.Nothing);
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);

            var page = new ViewUser(userQueries.Object, currentAuthenticatedUserProvider.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_GivenAuthenticateUserIsTheSameAsTheOneSelected_ExpectDataToBeSetAndPageResultReturnAndViewingSelfToBeSet()
        {
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(TestVariables.UserId, "email-address", "first-name",
                    "last-name", true, TestVariables.Now, null, TestVariables.Now, true, new List<Guid>(), null)));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var page = new ViewUser(userQueries.Object, currentAuthenticatedUserProvider.Object)
            {
                Id = TestVariables.AuthenticatedUserId,
            };
            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal("email-address", page.DetailedUser.EmailAddress);
            Assert.True(page.ViewingSelf);
        }

        [Fact]
        public async Task OnGetAsync_GivenUserExists_ExpectDataToBeSetAndPageResultReturn()
        {
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(TestVariables.UserId, "email-address", "first-name",
                    "last-name", true, TestVariables.Now, null, TestVariables.Now, true, new List<Guid>(), null)));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var page = new ViewUser(userQueries.Object, currentAuthenticatedUserProvider.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal("email-address", page.DetailedUser.EmailAddress);
            Assert.False(page.ViewingSelf);
        }
    }
}