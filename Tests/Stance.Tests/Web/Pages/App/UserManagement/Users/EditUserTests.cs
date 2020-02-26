// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
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
using Stance.Web.Infrastructure.Constants;
using Stance.Web.Infrastructure.PageModels;
using Stance.Web.Pages.App.UserManagement.Users;
using Xunit;

namespace Stance.Tests.Web.Pages.App.UserManagement.Users
{
    public class EditUserTests
    {
        [Fact]
        public async Task OnGetAsync_GivenPageModelIsNotNull_ExpectPageModelNotToBeOverridenAndPageResultReturn()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(Guid.NewGuid(), new string('*', 4), new string('*', 5),
                    new string('*', 6), true, DateTime.UtcNow, null, null)));

            var page = new EditUser(userQueries.Object, mediator.Object) { PageModel = new EditUser.Model() };
            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Null(page.PageModel.EmailAddress);
        }

        [Fact]
        public async Task OnGetAsync_GivenPageModelIsNull_ExpectPageModelToBeSetFromDatabaseAndPageResultReturn()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(Guid.NewGuid(), new string('*', 4), new string('*', 5),
                    new string('*', 6), true, DateTime.UtcNow, null, null)));

            var page = new EditUser(userQueries.Object, mediator.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal(new string('*', 4), page.PageModel.EmailAddress);
        }

        [Fact]
        public async Task OnGetAsync_GivenUserIsLockableAndIsLocked_ExpectLockedStatusToHaveCorrectMessage()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueries>();
            var whenLocked = DateTime.UtcNow;
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(Guid.NewGuid(), new string('*', 4), new string('*', 5),
                    new string('*', 6), true, DateTime.UtcNow, null, whenLocked)));

            var page = new EditUser(userQueries.Object, mediator.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal(whenLocked.ToString(), page.LockedStatus);
        }

        [Fact]
        public async Task OnGetAsync_GivenUserIsLockableButIsNotLocked_ExpectLockedStatusToHaveCorrectMessage()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(Guid.NewGuid(), new string('*', 4), new string('*', 5),
                    new string('*', 6), true, DateTime.UtcNow, null, null)));

            var page = new EditUser(userQueries.Object, mediator.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal("Not Locked", page.LockedStatus);
        }

        [Fact]
        public async Task OnGetAsync_GivenUserIsNotInSystem_ExpectNotFoundResultReturn()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe<DetailedUserModel>.Nothing);

            var page = new EditUser(userQueries.Object, mediator.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_GivenUserIsNotLockable_ExpectLockedStatusToHaveCorrectMessage()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueries>();

            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(Guid.NewGuid(), new string('*', 4), new string('*', 5),
                    new string('*', 6), false, DateTime.UtcNow, null, null)));

            var page = new EditUser(userQueries.Object, mediator.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal("User is not lockable", page.LockedStatus);
        }

        [Fact]
        public async Task
            OnGetAsync_GivenWhenLastAuthenticatedIsNotNull_ExpectAuthenticationStatusToHaveCorrectMessage()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueries>();
            var whenLastAuthenticated = DateTime.UtcNow;
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(Guid.NewGuid(), new string('*', 4), new string('*', 5),
                    new string('*', 6), true, DateTime.UtcNow, whenLastAuthenticated, null)));

            var page = new EditUser(userQueries.Object, mediator.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal(whenLastAuthenticated.ToString(), page.AuthenticationStatus);
        }

        [Fact]
        public async Task OnGetAsync_GivenWhenLastAuthenticatedIsNull_ExpectAuthenticationStatusToHaveCorrectMessage()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(Guid.NewGuid(), new string('*', 4), new string('*', 5),
                    new string('*', 6), true, DateTime.UtcNow, null, null)));

            var page = new EditUser(userQueries.Object, mediator.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal("Never Authenticated", page.AuthenticationStatus);
        }

        [Fact]
        public async Task OnPostAsync_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueries>();

            var page = new EditUser(userQueries.Object, mediator.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task
            OnPostAsync_GivenValidModelStateAndCommandDoesExecute_ExpectRedirectToPageResultToViewUserPage()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UpdateUserCoreDetailsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());
            var userQueries = new Mock<IUserQueries>();

            var page = new EditUser(userQueries.Object, mediator.Object) { PageModel = new EditUser.Model() };

            var result = await page.OnPostAsync();
            var pageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PageLocations.UserView, pageResult.PageName);
        }

        [Fact]
        public async Task
            OnPostAsync_GivenValidModelStateAndCommandDoesNotExecute_ExpectRedirectToPageResultAndPrgStateSetToFailed()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UpdateUserCoreDetailsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.SavingChanges)));
            var userQueries = new Mock<IUserQueries>();

            var page = new EditUser(userQueries.Object, mediator.Object) { PageModel = new EditUser.Model() };

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Failed, page.PrgState);
        }

        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var model = new EditUser.Model { EmailAddress = "a@b.com" };
            var validator = new EditUser.Validator();
            var result = validator.Validate(model);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenEmailAddressIsEmpty_ExpectValidationFailure()
        {
            var model = new EditUser.Model { EmailAddress = string.Empty };
            var validator = new EditUser.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenEmailAddressIsNotValidEmailAddress_ExpectValidationFailure()
        {
            var model = new EditUser.Model { EmailAddress = new string('*', 5) };
            var validator = new EditUser.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenEmailAddressIsNull_ExpectValidationFailure()
        {
            var model = new EditUser.Model { EmailAddress = null };
            var validator = new EditUser.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
        }
    }
}