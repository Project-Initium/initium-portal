// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Queries.Contracts.Static;
using Initium.Portal.Queries.Static.Models.User;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Pages.App.Profile;
using MaybeMonad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.App.Profile
{
    public class DetailsTests
    {
        [Fact]
        public async Task OnGetAsync_GivenNullPageModelAndNoProfile_ExpectNotFoundResult()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetProfileForCurrentUser())
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
            userQueries.Setup(x => x.GetProfileForCurrentUser()).ReturnsAsync(() =>
                Maybe.From(new ProfileModel("first-name", "last-name")));

            var page = new Details(mediator.Object, userQueries.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal("first-name", page.PageModel.FirstName);
            Assert.Equal("last-name", page.PageModel.LastName);
        }

        [Fact]
        public async Task OnGetAsync_GivenPageModelIsNotNull_ExpectPageResultAndPageModelDataSetWithoutDataCall()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueries>();

            var page = new Details(mediator.Object, userQueries.Object) { PageModel = new Details.Model() };

            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            userQueries.Verify(x => x.GetProfileForCurrentUser(), Times.Never);
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

        public class ValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var cmd = new Details.Model { FirstName = "first-name", LastName = "last-name" };
                var validator = new Details.Validator();
                var result = validator.Validate(cmd);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenLastNameIsEmpty_ExpectValidationFailure()
            {
                var cmd = new Details.Model { FirstName = "first-name", LastName = string.Empty };
                var validator = new Details.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "LastName");
            }

            [Fact]
            public void Validate_GivenLastNameIsNull_ExpectValidationFailure()
            {
                var cmd = new Details.Model { FirstName = "first-name", LastName = null };
                var validator = new Details.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "LastName");
            }

            [Fact]
            public void Validate_GivenFirstNameIsEmpty_ExpectValidationFailure()
            {
                var cmd = new Details.Model { FirstName = string.Empty, LastName = "last-name" };
                var validator = new Details.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "FirstName");
            }

            [Fact]
            public void Validate_GivenFirstNameIsNull_ExpectValidationFailure()
            {
                var cmd = new Details.Model { FirstName = null, LastName = "last-name" };
                var validator = new Details.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "FirstName");
            }
        }
    }
}