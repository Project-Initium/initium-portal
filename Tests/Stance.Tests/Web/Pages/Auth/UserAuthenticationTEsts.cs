// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.CommandResults.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.Constants;
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

            var page = new UserAuthentication(mediator.Object) { PageModel = new UserAuthentication.Model() };

            var result = await page.OnPost();

            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.InError, page.PrgState);
        }

        [Fact]
        public async Task
            OnPost_GivenCommandFailureAndReturnUrlIsNotEmpty_ExpectRedirectToPageResultToSamePageWithRedirectUrlAndPrgStateSet()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AuthenticateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                    Result.Fail<AuthenticateUserCommandResult, ErrorData>(new ErrorData(ErrorCodes.SavingChanges)));

            var page = new UserAuthentication(mediator.Object)
            {
                PageModel = new UserAuthentication.Model(),
                ReturnUrl = "some-url",
            };

            var result = await page.OnPost();

            var pageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.InError, page.PrgState);
            Assert.Equal("some-url", pageResult.RouteValues["ReturnUrl"]);
        }

        [Fact]
        public async Task OnPost_GivenCommandSuccess_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AuthenticateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                    Result.Ok<AuthenticateUserCommandResult, ErrorData>(
                        new AuthenticateUserCommandResult(Guid.NewGuid(), new string('*', 6), new string('*', 7), new string('*', 8))));

            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object,
            };

            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(),
                new ModelStateDictionary());
            var pageContext = new PageContext(actionContext);

            var page = new UserAuthentication(mediator.Object)
            {
                PageContext = pageContext,
                PageModel = new UserAuthentication.Model(),
            };

            var result = await page.OnPost();

            var pageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PageLocations.AppDashboard, pageResult.PageName);
            authServiceMock.Verify(
                x => x.SignInAsync(It.IsAny<HttpContext>(), CookieAuthenticationDefaults.AuthenticationScheme,
                    It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Once);
        }

        [Fact]
        public async Task OnPost_GivenCommandSuccessAndReturnUrlIsNotEmpty_ExpectLocalRedirectResult()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AuthenticateUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                    Result.Ok<AuthenticateUserCommandResult, ErrorData>(
                        new AuthenticateUserCommandResult(Guid.NewGuid(), new string('*', 6), new string('*', 7), new string('*', 8))));

            var authServiceMock = new Mock<IAuthenticationService>();
            authServiceMock
                .Setup(x => x.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(),
                    It.IsAny<AuthenticationProperties>()))
                .Returns(Task.CompletedTask);
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object,
            };

            var actionContext = new ActionContext(httpContext, new RouteData(), new PageActionDescriptor(),
                new ModelStateDictionary());
            var pageContext = new PageContext(actionContext);

            var page = new UserAuthentication(mediator.Object)
            {
                PageContext = pageContext,
                PageModel = new UserAuthentication.Model(),
                ReturnUrl = "some-url",
            };

            var result = await page.OnPost();

            var pageResult = Assert.IsType<LocalRedirectResult>(result);
            Assert.Equal("some-url", pageResult.Url);
            authServiceMock.Verify(
                x => x.SignInAsync(It.IsAny<HttpContext>(), CookieAuthenticationDefaults.AuthenticationScheme,
                    It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Once);
        }

        [Fact]
        public async Task OnPost_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();

            var page = new UserAuthentication(mediator.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPost();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task
            OnPost_GivenInvalidModelStateAndReturnUrlIsNotEmpty_ExpectRedirectToPageResultWithRedirectUrl()
        {
            var mediator = new Mock<IMediator>();

            var page = new UserAuthentication(mediator.Object);
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