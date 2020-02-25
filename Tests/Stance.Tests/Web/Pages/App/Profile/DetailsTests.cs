// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.Commands.UserAggregate;
using Stance.Queries.Contracts;
using Stance.Queries.Models.User;
using Stance.Web.Infrastructure.PageModels;
using Stance.Web.Pages.App.Profile;
using Xunit;

namespace Stance.Tests.Web.Pages.App.Profile
{
    public class DetailsTests
    {
        [Fact]
        public async Task OnGetAsync_GivenNullPageModelAndNoProfile_ExpectNotFoundResult()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetProfileForCurrentUser(It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => Maybe<ProfileModel>.Nothing);

            var page = new Details(mediator.Object, userQueries.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_GivenNullPageModelAndProfile_ExpectPageResultAndPageModelDataSet()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetProfileForCurrentUser(It.IsAny<CancellationToken>())).ReturnsAsync(() =>
                Maybe.From(new ProfileModel(new string('*', 5), new string('*', 6))));

            var page = new Details(mediator.Object, userQueries.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal(new string('*', 5), page.PageModel.FirstName);
            Assert.Equal(new string('*', 6), page.PageModel.LastName);
        }

        [Fact]
        public async Task OnGetAsync_GivenPageModelIsNotNull_ExpectPageResultAndPageModelDataSetWithoutDataCall()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueries>();

            var page = new Details(mediator.Object, userQueries.Object) { PageModel = new Details.Model() };

            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            userQueries.Verify(x => x.GetProfileForCurrentUser(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task OnPostAsync_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueries>();

            var page = new Details(mediator.Object, userQueries.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_GivenValidModelStateAndCommandDoesNotExecute_ExpectRedirectToPageResultAndPrgStateSetToFailed()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UpdateProfileCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.SavingChanges)));
            var userQueries = new Mock<IUserQueries>();

            var page = new Details(mediator.Object, userQueries.Object) { PageModel = new Details.Model() };

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Failed, page.PrgState);
        }

        [Fact]
        public async Task OnPostAsync_GivenValidModelStateAndCommandExecutes_ExpectRedirectToPageResultAndPrgStateSetToSuccess()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UpdateProfileCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());
            var userQueries = new Mock<IUserQueries>();

            var page = new Details(mediator.Object, userQueries.Object) { PageModel = new Details.Model() };

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Success, page.PrgState);
        }
    }
}