// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Queries.Contracts.Static;
using Initium.Portal.Queries.Static.Models.Role;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Pages.App.UserManagement.Users;
using MaybeMonad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.App.UserManagement.Users
{
    public class CreateUserTests
    {
        [Fact]
        public async Task OnGetAsync_GivenThereAreNoRoles_ExpectEmptyList()
        {
            var mediator = new Mock<IMediator>();
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.GetSimpleRoles())
                .ReturnsAsync(Maybe<List<SimpleRoleModel>>.Nothing);

            var page = new CreateUser(mediator.Object, roleQueries.Object);
            await page.OnGetAsync();

            Assert.Empty(page.AvailableRoles);
        }

        [Fact]
        public async Task OnGetAsync_GivenThereAreRoles_ExpectPopulatedList()
        {
            var mediator = new Mock<IMediator>();
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.GetSimpleRoles())
                .ReturnsAsync(Maybe.From(new List<SimpleRoleModel>
                {
                    new SimpleRoleModel(TestVariables.RoleId, "name"),
                }));

            var page = new CreateUser(mediator.Object, roleQueries.Object);
            await page.OnGetAsync();

            Assert.Single(page.AvailableRoles);
        }

        [Fact]
        public async Task OnPostAsync_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();
            var roleQueries = new Mock<IRoleQueries>();

            var page = new CreateUser(mediator.Object, roleQueries.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task
            OnPostAsync_GivenValidModelStateAndCommandDoesNotExecute_ExpectRedirectToPageResultAndPrgStateSetToFailed()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Fail<CreateUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.SavingChanges)));
            var roleQueries = new Mock<IRoleQueries>();

            var page = new CreateUser(mediator.Object, roleQueries.Object) { PageModel = new CreateUser.Model() };

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Failed, page.PrgState);
        }

        [Fact]
        public async Task
            OnPostAsync_GivenValidModelStateAndCommandExecutes_ExpectRedirectToPageResultAndPrgStateSetToSuccess()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CreateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Result.Ok<CreateUserCommandResult, ErrorData>(new CreateUserCommandResult(TestVariables.UserId)));

            var roleQueries = new Mock<IRoleQueries>();

            var page = new CreateUser(mediator.Object, roleQueries.Object) { PageModel = new CreateUser.Model() };

            var result = await page.OnPostAsync();
            var pageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PageLocations.UserView, pageResult.PageName);
        }

        public class ValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var model = new CreateUser.Model { FirstName = "first-name", LastName = "last-name", EmailAddress = "a@b.com" };
                var validator = new CreateUser.Validator();
                var result = validator.Validate(model);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenEmailAddressIsEmpty_ExpectValidationFailure()
            {
                var model = new CreateUser.Model { FirstName = "first-name", LastName = "last-name", EmailAddress = string.Empty };
                var validator = new CreateUser.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }

            [Fact]
            public void Validate_GivenEmailAddressIsNotValidEmailAddress_ExpectValidationFailure()
            {
                var model = new CreateUser.Model { FirstName = "first-name", LastName = "last-name", EmailAddress = new string('*', 5) };
                var validator = new CreateUser.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }

            [Fact]
            public void Validate_GivenEmailAddressIsNull_ExpectValidationFailure()
            {
                var model = new CreateUser.Model { FirstName = "first-name", LastName = "last-name", EmailAddress = null };
                var validator = new CreateUser.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }

            [Fact]
            public void Validate_GivenLastNameIsEmpty_ExpectValidationFailure()
            {
                var cmd = new CreateUser.Model { FirstName = "first-name", LastName = string.Empty, EmailAddress = "a@b.com" };
                var validator = new CreateUser.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "LastName");
            }

            [Fact]
            public void Validate_GivenLastNameIsNull_ExpectValidationFailure()
            {
                var cmd = new CreateUser.Model { FirstName = "first-name", LastName = null, EmailAddress = "a@b.com" };
                var validator = new CreateUser.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "LastName");
            }

            [Fact]
            public void Validate_GivenFirstNameIsEmpty_ExpectValidationFailure()
            {
                var cmd = new CreateUser.Model { FirstName = string.Empty, LastName = "last-name", EmailAddress = "a@b.com" };
                var validator = new CreateUser.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "FirstName");
            }

            [Fact]
            public void Validate_GivenFirstNameIsNull_ExpectValidationFailure()
            {
                var cmd = new CreateUser.Model { FirstName = null, LastName = "last-name", EmailAddress = "a@b.com" };
                var validator = new CreateUser.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "FirstName");
            }
        }
    }
}