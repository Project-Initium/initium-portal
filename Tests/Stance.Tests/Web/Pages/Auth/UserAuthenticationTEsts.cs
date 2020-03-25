// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.CommandResults.UserAggregate;
using Stance.Domain.Commands.UserAggregate;

using Stance.Web.Infrastructure.Contracts;
using Stance.Web.Infrastructure.PageModels;
using Stance.Web.Pages.Auth;
using Xunit;

namespace Stance.Tests.Web.Pages.Auth
{
    public class UserAuthenticationTests
    {
        [Fact]
        public async Task OnPost_GivenCommandFailure_ExpectRedirectToPageResultToSamePageAndPrgStateSet()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AuthenticateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                    Result.Fail<AuthenticateUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.SavingChanges)));
            var authenticationService = new Mock<IAuthenticationService>();

            var page = new UserAuthentication(mediator.Object, authenticationService.Object) { PageModel = new UserAuthentication.Model() };

            var result = await page.OnPost();

            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Failed, page.PrgState);
        }

        [Fact]
        public async Task
            OnPost_GivenCommandFailureAndReturnUrlIsNotEmpty_ExpectRedirectToPageResultToSamePageWithRedirectUrlAndPrgStateSet()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AuthenticateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                    Result.Fail<AuthenticateUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.SavingChanges)));
            var authenticationService = new Mock<IAuthenticationService>();

            var page = new UserAuthentication(mediator.Object, authenticationService.Object)
            {
                PageModel = new UserAuthentication.Model(),
                ReturnUrl = "some-url",
            };

            var result = await page.OnPost();

            var pageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Failed, page.PrgState);
            Assert.Equal("some-url", pageResult.RouteValues["ReturnUrl"]);
        }

        [Fact]
        public async Task OnPost_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();

            var authServiceMock = new Mock<IAuthenticationService>();

            var page = new UserAuthentication(mediator.Object, authServiceMock.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPost();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task
            OnPost_GivenInvalidModelStateAndReturnUrlIsNotEmpty_ExpectRedirectToPageResultWithRedirectUrl()
        {
            var mediator = new Mock<IMediator>();
            var authServiceMock = new Mock<IAuthenticationService>();

            var page = new UserAuthentication(mediator.Object, authServiceMock.Object);
            page.ModelState.AddModelError("Error", "Error");

            page.ReturnUrl = "some-url";

            var result = await page.OnPost();

            var pageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("some-url", pageResult.RouteValues["ReturnUrl"]);
        }

        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var model = new UserAuthentication.Model { EmailAddress = "a@b.com", Password = new string('*', 6) };
            var validator = new UserAuthentication.Validator();
            var result = validator.Validate(model);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenEmailAddressIsEmpty_ExpectValidationFailure()
        {
            var model = new UserAuthentication.Model { EmailAddress = string.Empty, Password = new string('*', 6) };
            var validator = new UserAuthentication.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenEmailAddressIsNotValidEmailAddress_ExpectValidationFailure()
        {
            var model = new UserAuthentication.Model { EmailAddress = new string('*', 5), Password = new string('*', 6) };
            var validator = new UserAuthentication.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenEmailAddressIsNull_ExpectValidationFailure()
        {
            var model = new UserAuthentication.Model { EmailAddress = null, Password = new string('*', 6) };
            var validator = new UserAuthentication.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenPasswordIsEmpty_ExpectValidationFailure()
        {
            var model = new UserAuthentication.Model { EmailAddress = "a@b.com", Password = string.Empty };
            var validator = new UserAuthentication.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "Password");
        }

        [Fact]
        public void Validate_GivenPasswordIsNull_ExpectValidationFailure()
        {
            var model = new UserAuthentication.Model { EmailAddress = "a@b.com", Password = null };
            var validator = new UserAuthentication.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "Password");
        }
    }
}