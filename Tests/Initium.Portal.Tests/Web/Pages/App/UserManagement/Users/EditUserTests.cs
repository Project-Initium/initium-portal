// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models.Role;
using Initium.Portal.Queries.Models.User;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Pages.App.UserManagement.Users;
using MaybeMonad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.App.UserManagement.Users
{
    public class EditUserTests
    {
        [Fact]
        public async Task OnGetAsync_GivenNoUserIsAuthenticated_ExpectNotFoundResult()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueryService>();
            var roleQueries = new Mock<IRoleQueryService>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);

            var page = new EditUser(userQueries.Object, mediator.Object, roleQueries.Object, currentAuthenticatedUserProvider.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_GivenAttemptToEditSelf_ExpectNotFoundResult()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueryService>();
            var roleQueries = new Mock<IRoleQueryService>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var page = new EditUser(userQueries.Object, mediator.Object, roleQueries.Object, currentAuthenticatedUserProvider.Object)
            {
                Id = TestVariables.AuthenticatedUserId,
            };
            var result = await page.OnGetAsync();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_GivenPageModelIsNotNull_ExpectPageModelNotToBeOverridenAndPageResultReturn()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueryService>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(TestVariables.UserId, "email-address", "first-name",
                    "last-name", true, TestVariables.Now, null, null, true, new List<Guid>(), null)));
            var roleQueries = new Mock<IRoleQueryService>();
            roleQueries.Setup(s => s.GetSimpleRoles())
                .ReturnsAsync(() => Maybe.From(new List<SimpleRoleModel>()));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var page = new EditUser(userQueries.Object, mediator.Object, roleQueries.Object, currentAuthenticatedUserProvider.Object)
                { PageModel = new EditUser.Model() };
            var tempDataDictionary = new Mock<ITempDataDictionary>();
            page.TempData = tempDataDictionary.Object;

            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Null(page.PageModel.EmailAddress);
        }

        [Fact]
        public async Task OnGetAsync_GivenPageModelIsNull_ExpectPageModelToBeSetFromDatabaseAndPageResultReturn()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueryService>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(TestVariables.UserId, "email-address", "first-name",
                    "last-name", true, TestVariables.Now, null, null, true, new List<Guid>(), null)));
            var roleQueries = new Mock<IRoleQueryService>();
            roleQueries.Setup(s => s.GetSimpleRoles())
                .ReturnsAsync(() => Maybe.From(new List<SimpleRoleModel>
                {
                    new SimpleRoleModel(TestVariables.RoleId, "name"),
                }));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var page = new EditUser(userQueries.Object, mediator.Object, roleQueries.Object, currentAuthenticatedUserProvider.Object);
            var tempDataDictionary = new Mock<ITempDataDictionary>();
            page.TempData = tempDataDictionary.Object;

            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal("email-address", page.PageModel.EmailAddress);
            Assert.Equal("first-name last-name", page.Name);
            Assert.Equal(TestVariables.Now, page.WhenCreated);
            Assert.Single(page.AvailableRoles);
        }

        [Fact]
        public async Task OnGetAsync_GivenUserIsLockableAndIsLocked_ExpectLockedStatusToHaveCorrectMessage()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueryService>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(TestVariables.UserId, "email-address", "first-name",
                    "last-name", true, TestVariables.Now, null, TestVariables.Now, true, new List<Guid>(), null)));
            var roleQueries = new Mock<IRoleQueryService>();
            roleQueries.Setup(s => s.GetSimpleRoles())
                .ReturnsAsync(() => Maybe.From(new List<SimpleRoleModel>()));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var page = new EditUser(userQueries.Object, mediator.Object, roleQueries.Object, currentAuthenticatedUserProvider.Object);
            var tempDataDictionary = new Mock<ITempDataDictionary>();
            page.TempData = tempDataDictionary.Object;

            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal(TestVariables.Now.ToString(CultureInfo.CurrentCulture), page.LockedStatus);
        }

        [Fact]
        public async Task OnGetAsync_GivenUserIsLockableButIsNotLocked_ExpectLockedStatusToHaveCorrectMessage()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueryService>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(TestVariables.UserId, "email-address", "first-name",
                    "last-name", true, TestVariables.Now, null, null, true, new List<Guid>(), null)));
            var roleQueries = new Mock<IRoleQueryService>();
            roleQueries.Setup(s => s.GetSimpleRoles())
                .ReturnsAsync(() => Maybe.From(new List<SimpleRoleModel>()));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var page = new EditUser(userQueries.Object, mediator.Object, roleQueries.Object, currentAuthenticatedUserProvider.Object);
            var tempDataDictionary = new Mock<ITempDataDictionary>();
            page.TempData = tempDataDictionary.Object;

            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal("Not Locked", page.LockedStatus);
        }

        [Fact]
        public async Task OnGetAsync_GivenUserIsNotInSystem_ExpectNotFoundResultReturn()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueryService>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe<DetailedUserModel>.Nothing);
            var roleQueries = new Mock<IRoleQueryService>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var page = new EditUser(userQueries.Object, mediator.Object, roleQueries.Object, currentAuthenticatedUserProvider.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_GivenUserIsNotLockable_ExpectLockedStatusToHaveCorrectMessage()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueryService>();

            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(TestVariables.UserId, "email-address", "first-name",
                    "last-name", false, TestVariables.Now, null, null, true, new List<Guid>(), null)));
            var roleQueries = new Mock<IRoleQueryService>();
            roleQueries.Setup(s => s.GetSimpleRoles())
                .ReturnsAsync(() => Maybe.From(new List<SimpleRoleModel>()));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var page = new EditUser(userQueries.Object, mediator.Object, roleQueries.Object, currentAuthenticatedUserProvider.Object);
            var tempDataDictionary = new Mock<ITempDataDictionary>();
            page.TempData = tempDataDictionary.Object;

            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal("User is not lockable", page.LockedStatus);
        }

        [Fact]
        public async Task
            OnGetAsync_GivenWhenLastAuthenticatedIsNotNull_ExpectAuthenticationStatusToHaveCorrectMessage()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueryService>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(TestVariables.UserId, "email-address", "first-name",
                    "last-name", false, TestVariables.Now, TestVariables.Now.AddMinutes(30), null, true,
                    new List<Guid>(), null)));
            var roleQueries = new Mock<IRoleQueryService>();
            roleQueries.Setup(s => s.GetSimpleRoles())
                .ReturnsAsync(() => Maybe.From(new List<SimpleRoleModel>()));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var page = new EditUser(userQueries.Object, mediator.Object, roleQueries.Object, currentAuthenticatedUserProvider.Object);
            var tempDataDictionary = new Mock<ITempDataDictionary>();
            page.TempData = tempDataDictionary.Object;

            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal(TestVariables.Now.AddMinutes(30).ToString(CultureInfo.CurrentCulture), page.AuthenticationStatus);
        }

        [Fact]
        public async Task OnGetAsync_GivenWhenLastAuthenticatedIsNull_ExpectAuthenticationStatusToHaveCorrectMessage()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueryService>();
            userQueries.Setup(x => x.GetDetailsOfUserById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe.From(new DetailedUserModel(TestVariables.UserId, "email-address", "first-name",
                    "last-name", true, TestVariables.Now, null, null, true, new List<Guid>(), null)));
            var roleQueries = new Mock<IRoleQueryService>();
            roleQueries.Setup(s => s.GetSimpleRoles())
                .ReturnsAsync(() => Maybe.From(new List<SimpleRoleModel>()));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var page = new EditUser(userQueries.Object, mediator.Object, roleQueries.Object, currentAuthenticatedUserProvider.Object);
            var tempDataDictionary = new Mock<ITempDataDictionary>();
            page.TempData = tempDataDictionary.Object;

            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal("Never Authenticated", page.AuthenticationStatus);
        }

        [Fact]
        public async Task OnPostAsync_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();
            var userQueries = new Mock<IUserQueryService>();
            var roleQueries = new Mock<IRoleQueryService>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var page = new EditUser(userQueries.Object, mediator.Object, roleQueries.Object, currentAuthenticatedUserProvider.Object);
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
            var userQueries = new Mock<IUserQueryService>();
            var roleQueries = new Mock<IRoleQueryService>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var page = new EditUser(userQueries.Object, mediator.Object, roleQueries.Object, currentAuthenticatedUserProvider.Object)
                { PageModel = new EditUser.Model() };

            var result = await page.OnPostAsync();
            var pageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(CorePageLocations.UserView, pageResult.PageName);
        }

        [Fact]
        public async Task
            OnPostAsync_GivenValidModelStateAndCommandDoesNotExecute_ExpectRedirectToPageResultAndPrgStateSetToFailed()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UpdateUserCoreDetailsCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.SavingChanges)));
            var userQueries = new Mock<IUserQueryService>();
            var roleQueries = new Mock<IRoleQueryService>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var page = new EditUser(userQueries.Object, mediator.Object, roleQueries.Object, currentAuthenticatedUserProvider.Object)
                { PageModel = new EditUser.Model() };

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Failed, page.PrgState);
        }

        public class ValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var model = new EditUser.Model
                {
                    FirstName = "first-name", LastName = "last-name", EmailAddress = "a@b.com",
                    UserId = TestVariables.UserId,
                };
                var validator = new EditUser.Validator();
                var result = validator.Validate(model);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenEmailAddressIsEmpty_ExpectValidationFailure()
            {
                var model = new EditUser.Model
                {
                    FirstName = "first-name", LastName = "last-name", EmailAddress = string.Empty,
                    UserId = TestVariables.UserId,
                };
                var validator = new EditUser.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }

            [Fact]
            public void Validate_GivenEmailAddressIsNotValidEmailAddress_ExpectValidationFailure()
            {
                var model = new EditUser.Model
                {
                    FirstName = "first-name", LastName = "last-name", EmailAddress = "email-address",
                    UserId = TestVariables.UserId,
                };
                var validator = new EditUser.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }

            [Fact]
            public void Validate_GivenEmailAddressIsNull_ExpectValidationFailure()
            {
                var model = new EditUser.Model
                {
                    FirstName = "first-name", LastName = "last-name", EmailAddress = null,
                    UserId = TestVariables.UserId,
                };
                var validator = new EditUser.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }

            [Fact]
            public void Validate_GivenFirstNameIsEmpty_ExpectValidationFailure()
            {
                var cmd = new EditUser.Model
                {
                    FirstName = string.Empty, LastName = "last-name", EmailAddress = "a@b.com",
                    UserId = TestVariables.UserId,
                };
                var validator = new EditUser.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "FirstName");
            }

            [Fact]
            public void Validate_GivenFirstNameIsNull_ExpectValidationFailure()
            {
                var cmd = new EditUser.Model
                {
                    FirstName = null, LastName = "last-name", EmailAddress = "a@b.com",
                    UserId = TestVariables.UserId,
                };
                var validator = new EditUser.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "FirstName");
            }

            [Fact]
            public void Validate_GivenLastNameIsEmpty_ExpectValidationFailure()
            {
                var cmd = new EditUser.Model
                {
                    FirstName = "first-name", LastName = string.Empty, EmailAddress = "a@b.com",
                    UserId = TestVariables.UserId,
                };
                var validator = new EditUser.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "LastName");
            }

            [Fact]
            public void Validate_GivenLastNameIsNull_ExpectValidationFailure()
            {
                var cmd = new EditUser.Model
                {
                    FirstName = "first-name", LastName = null, EmailAddress = "a@b.com", UserId = TestVariables.UserId,
                };
                var validator = new EditUser.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "LastName");
            }

            [Fact]
            public void Validate_GivenUserIdIsEmpty_ExpectValidationFailure()
            {
                var model = new EditUser.Model
                    { FirstName = "first-name", LastName = "last-name", EmailAddress = "a@b.com", UserId = Guid.Empty };
                var validator = new EditUser.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "UserId");
            }
        }
    }
}