// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using Moq;
using Stance.Queries.Contracts;
using Stance.Queries.Models.User;
using Stance.Web.Pages.App.UserManagement.Users;
using Xunit;

namespace Stance.Tests.Web.Pages.App.UserManagement.Users
{
    public class ListUsersTests
    {
        [Fact]
        public async Task OnGetAsync_GivenAuthenticationStatsAreLoaded_ExpectedDataToBePopulated()
        {
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetAuthenticationStats(It.IsAny<CancellationToken>())).ReturnsAsync(() =>
                Maybe.From(new AuthenticationStatsModel(1, 2, 3, 4)));

            var page = new ListUsers(userQueries.Object);
            await page.OnGetAsync();
            Assert.Equal(1, page.TotalNewUsers);
            Assert.Equal(2, page.TotalActiveUsers);
            Assert.Equal(3, page.TotalLogins);
            Assert.Equal(4, page.TotalLockedAccounts);
        }

        [Fact]
        public async Task OnGetAsync_GivenAuthenticationStatsAreLoaded_ExpectedDataToHaveZeroData()
        {
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetAuthenticationStats(It.IsAny<CancellationToken>())).ReturnsAsync(() =>
                Maybe<AuthenticationStatsModel>.Nothing);

            var page = new ListUsers(userQueries.Object);
            await page.OnGetAsync();
            Assert.Equal(0, page.TotalNewUsers);
            Assert.Equal(0, page.TotalActiveUsers);
            Assert.Equal(0, page.TotalLogins);
            Assert.Equal(0, page.TotalLockedAccounts);
        }
    }
}