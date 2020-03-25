// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.Commands.UserAggregate;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static.Models;
using Stance.Web.Infrastructure.Constants;
using Stance.Web.Infrastructure.PageModels;
using Stance.Web.Pages.FirstRun;
using Xunit;

namespace Stance.Tests.Web.Pages.FirstRun
{
    public class InitialUserSetupTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var model = new InitialUserSetup.Model { EmailAddress = "a@b.com", Password = new string('*', 6), PasswordConfirmation = new string('*', 6) };
            var validator = new InitialUserSetup.Validator();
            var result = validator.Validate(model);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenEmailAddressIsEmpty_ExpectValidationFailure()
        {
            var model = new InitialUserSetup.Model { EmailAddress = string.Empty, Password = new string('*', 6), PasswordConfirmation = new string('*', 6) };
            var validator = new InitialUserSetup.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenEmailAddressIsNotValidEmailAddress_ExpectValidationFailure()
        {
            var model = new InitialUserSetup.Model { EmailAddress = new string('*', 5), Password = new string('*', 6), PasswordConfirmation = new string('*', 6) };
            var validator = new InitialUserSetup.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenEmailAddressIsNull_ExpectValidationFailure()
        {
            var model = new InitialUserSetup.Model { EmailAddress = null, Password = new string('*', 6), PasswordConfirmation = new string('*', 6) };
            var validator = new InitialUserSetup.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenPasswordIsEmpty_ExpectValidationFailure()
        {
            var model = new InitialUserSetup.Model { EmailAddress = "a@b.com", Password = string.Empty, PasswordConfirmation = string.Empty };
            var validator = new InitialUserSetup.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "Password");
        }

        [Fact]
        public void Validate_GivenPasswordIsNull_ExpectValidationFailure()
        {
            var model = new InitialUserSetup.Model { EmailAddress = "a@b.com", Password = null, PasswordConfirmation = null };
            var validator = new InitialUserSetup.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "Password");
        }

        [Fact]
        public void Validate_GivenPasswordDoesNotMatchPasswordConfirmation_ExpectValidationFailure()
        {
            var model = new InitialUserSetup.Model { EmailAddress = "a@b.com", Password = new string('*', 6), PasswordConfirmation = null };
            var validator = new InitialUserSetup.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "PasswordConfirmation");
        }

        [Fact]
        public async Task OnGet_GivenUserInSystem_ExpectNotFoundResult()
        {
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.CheckForPresenceOfAnyUser())
                .ReturnsAsync(() => new StatusCheckModel(true));
            var mediator = new Mock<IMediator>();

            var page = new InitialUserSetup(userQueries.Object, mediator.Object);

            var result = await page.OnGet();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGet_GivenNoUserInSystem_ExpectPageResult()
        {
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.CheckForPresenceOfAnyUser())
                .ReturnsAsync(() => new StatusCheckModel(false));
            var mediator = new Mock<IMediator>();

            var page = new InitialUserSetup(userQueries.Object, mediator.Object);

            var result = await page.OnGet();
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public async Task OnPost_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var userQueries = new Mock<IUserQueries>();
            var mediator = new Mock<IMediator>();

            var page = new InitialUserSetup(userQueries.Object, mediator.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPost();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPost_GivenCommandFailure_ExpectRedirectToPageResultToSamePageAndPrgStateSet()
        {
            var userQueries = new Mock<IUserQueries>();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CreateInitialUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ResultWithError.Fail<ErrorData>(new ErrorData(ErrorCodes.SavingChanges)));

            var page = new InitialUserSetup(userQueries.Object, mediator.Object)
            {
                PageModel = new InitialUserSetup.Model(),
            };

            var result = await page.OnPost();
            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Failed, page.PrgState);
        }

        [Fact]
        public async Task OnPost_GivenCommandSuccess_ExpectRedirectToPageResult()
        {
            var userQueries = new Mock<IUserQueries>();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CreateInitialUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ResultWithError.Ok<ErrorData>());

            var page = new InitialUserSetup(userQueries.Object, mediator.Object)
            {
                PageModel = new InitialUserSetup.Model(),
            };

            var result = await page.OnPost();
            var redirectToPageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PageLocations.FirstRunSetupCompleted, redirectToPageResult.PageName);
        }
    }
}