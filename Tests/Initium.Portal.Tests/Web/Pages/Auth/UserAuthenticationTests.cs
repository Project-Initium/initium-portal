// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Fido2NetLib;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.Contracts;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Pages.Auth;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.Auth
{
    public class UserAuthenticationTests
    {
        [Fact]
        public async Task OnPostAsync_GivenCommandFailure_ExpectRedirectToPageResultToSamePageAndPrgStateSet()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AuthenticateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                    Result.Fail<AuthenticateUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.SavingChanges)));
            var authenticationService = new Mock<IAuthenticationService>();

            var page = new UserAuthentication(mediator.Object, authenticationService.Object) { PageModel = new UserAuthentication.Model() };

            var result = await page.OnPostAsync();

            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Failed, page.PrgState);
        }

        [Fact]
        public async Task
            OnPostAsync_GivenCommandFailureAndReturnUrlIsNotEmpty_ExpectRedirectToPageResultToSamePageWithRedirectUrlAndPrgStateSet()
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

            var result = await page.OnPostAsync();

            var pageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Failed, page.PrgState);
            Assert.Equal("some-url", pageResult.RouteValues["ReturnUrl"]);
        }

        [Fact]
        public async Task OnPostAsync_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();

            var authServiceMock = new Mock<IAuthenticationService>();

            var page = new UserAuthentication(mediator.Object, authServiceMock.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task
            OnPostAsync_GivenInvalidModelStateAndReturnUrlIsNotEmpty_ExpectRedirectToPageResultWithRedirectUrl()
        {
            var mediator = new Mock<IMediator>();
            var authServiceMock = new Mock<IAuthenticationService>();

            var page = new UserAuthentication(mediator.Object, authServiceMock.Object);
            page.ModelState.AddModelError("Error", "Error");

            page.ReturnUrl = "some-url";

            var result = await page.OnPostAsync();

            var pageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal("some-url", pageResult.RouteValues["ReturnUrl"]);
        }

        [Fact]
        public async Task
            OnPostAsync_GivenSuccessfulExecutionAndMfaIsDevice_ExpectPartialAuthenticationAndRedirectDeviceMfaPage()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AuthenticateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                    Result.Ok<AuthenticateUserCommandResult, ErrorData>(new AuthenticateUserCommandResult(TestVariables.UserId, MfaProvider.Device, new AssertionOptions())));
            var authenticationService = new Mock<IAuthenticationService>();

            var page = new UserAuthentication(mediator.Object, authenticationService.Object)
            {
                PageModel = new UserAuthentication.Model(),
                ReturnUrl = "some-url",
            };
            var tempDataDictionary = new Mock<ITempDataDictionary>();
            page.TempData = tempDataDictionary.Object;

            var result = Assert.IsType<RedirectToPageResult>(await page.OnPostAsync());
            Assert.Equal(PageLocations.AuthDeviceMfa, result.PageName);
        }

        [Fact]
        public async Task
            OnPostAsync_GivenSuccessfulExecutionAndMfaIsApp_ExpectPartialAuthenticationAndRedirectAppMfaPage()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AuthenticateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                    Result.Ok<AuthenticateUserCommandResult, ErrorData>(new AuthenticateUserCommandResult(TestVariables.UserId, BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaAppCode, MfaProvider.App)));
            var authenticationService = new Mock<IAuthenticationService>();

            var page = new UserAuthentication(mediator.Object, authenticationService.Object)
            {
                PageModel = new UserAuthentication.Model(),
                ReturnUrl = "some-url",
            };

            var result = Assert.IsType<RedirectToPageResult>(await page.OnPostAsync());
            Assert.Equal(PageLocations.AuthAppMfa, result.PageName);
        }

        [Fact]
        public async Task
            OnPostAsync_GivenSuccessfulExecutionAndMfaIsEmail_ExpectPartialAuthenticationAndRedirectEmailMfaPage()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AuthenticateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                    Result.Ok<AuthenticateUserCommandResult, ErrorData>(new AuthenticateUserCommandResult(TestVariables.UserId, BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaEmailCode, MfaProvider.Email)));
            var authenticationService = new Mock<IAuthenticationService>();

            var page = new UserAuthentication(mediator.Object, authenticationService.Object)
            {
                PageModel = new UserAuthentication.Model(),
                ReturnUrl = "some-url",
            };

            var result = Assert.IsType<RedirectToPageResult>(await page.OnPostAsync());
            Assert.Equal(PageLocations.AuthEmailMfa, result.PageName);
        }

        public class ValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var model = new UserAuthentication.Model { EmailAddress = "a@b.com", Password = "password" };
                var validator = new UserAuthentication.Validator();
                var result = validator.Validate(model);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenEmailAddressIsEmpty_ExpectValidationFailure()
            {
                var model = new UserAuthentication.Model { EmailAddress = string.Empty, Password = "password" };
                var validator = new UserAuthentication.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }

            [Fact]
            public void Validate_GivenEmailAddressIsNotValidEmailAddress_ExpectValidationFailure()
            {
                var model = new UserAuthentication.Model { EmailAddress = "email-address", Password = "password" };
                var validator = new UserAuthentication.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "EmailAddress");
            }

            [Fact]
            public void Validate_GivenEmailAddressIsNull_ExpectValidationFailure()
            {
                var model = new UserAuthentication.Model { EmailAddress = null, Password = "password" };
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
}