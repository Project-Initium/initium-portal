// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using Stance.Queries.Contracts;
using Stance.Queries.Models.User;
using Stance.Web.Pages.App.UserManagement.Users;
using Xunit;

namespace Stance.Tests.Web.Pages.App.UserManagement.Users
{
    public class ViewUserTests
    {
        [Fact]
        public async Task OnGetAsync_GivenUserDoesNotExist_ExpectNotFoundResultReturn()
        {
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe<DetailedUserModel>.Nothing);

            var page = new ViewUser(userQueries.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_GivenUserExists_ExpectDataToBeSetAndPageResultReturn()
        {
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(Guid.NewGuid(), new string('*', 4), new string('*', 5),
                    new string('*', 6), true, DateTime.UtcNow, null, null, true, new List<Guid>())));

            var page = new ViewUser(userQueries.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal(new string('*', 4), page.DetailedUser.EmailAddress);
        }
    }
}