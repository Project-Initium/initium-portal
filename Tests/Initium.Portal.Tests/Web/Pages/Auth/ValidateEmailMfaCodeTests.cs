// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Fido2NetLib;
using Initium.Portal.Core;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.Contracts;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Pages.Auth;
using MaybeMonad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.Auth
{
    public class ValidateEmailMfaCodeTests
    {
        [Fact]
        public void OnGet_GivenNoUserIsAuthenticated_ExpectNoMfaTypesSet()
        {
            var mediator = new Mock<IMediator>();
            var authenticationService = new Mock<IAuthenticationService>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);

            var page = new ValidateEmailMfaCode(mediator.Object, authenticationService.Object,
                currentAuthenticatedUserProvider.Object);

            page.OnGet();

            Assert.False(page.HasApp);
            Assert.False(page.HasDevice);
        }

        [Fact]
        public void OnGet_GivenUserIsAuthenticated_ExpectMfaTypesToBeSet()
        {
            var mediator = new Mock<IMediator>();
            var authenticationService = new Mock<IAuthenticationService>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(
                    new UnauthenticatedUser(
                        TestVariables.UserId, MfaProvider.App | MfaProvider.Device) as ISystemUser));

            var page = new ValidateEmailMfaCode(mediator.Object, authenticationService.Object,
                currentAuthenticatedUserProvider.Object);

            page.OnGet();

            Assert.True(page.HasApp);
            Assert.True(page.HasDevice);
        }

        [Fact]
        public void OnGet_GivenUserIsAuthenticatedButIsNotRightType_ExpectNoMfaTypesSet()
        {
            var mediator = new Mock<IMediator>();
            var authenticationService = new Mock<IAuthenticationService>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new Mock<ISystemUser>().Object));

            var page = new ValidateEmailMfaCode(mediator.Object, authenticationService.Object,
                currentAuthenticatedUserProvider.Object);

            page.OnGet();

            Assert.False(page.HasApp);
            Assert.False(page.HasDevice);
        }

        [Fact]
        public async Task OnPost_GivenCommandFails_ExpectRedirectToPageResultToSamePageAndPrgStateSet()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.Send(
                    It.IsAny<ValidateEmailMfaCodeAgainstCurrentUserCommand>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(() =>
                    Result.Fail<ValidateEmailMfaCodeAgainstCurrentUserCommandResult, ErrorData>(
                        new ErrorData(ErrorCodes.SavingChanges)));
            var authenticationService = new Mock<IAuthenticationService>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var page = new ValidateEmailMfaCode(mediator.Object, authenticationService.Object,
                currentAuthenticatedUserProvider.Object) { PageModel = new ValidateEmailMfaCode.Model() };

            var result = await page.OnPost();

            Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Failed, page.PrgState);
        }

        [Fact]
        public async Task
            OnPost_GivenCommandSucceedsWithNoReturnUrl_ExpectUserToBeAuthenticatedAndRedirectedToDashboard()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.Send(
                    It.IsAny<ValidateEmailMfaCodeAgainstCurrentUserCommand>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(() =>
                    Result.Ok<ValidateEmailMfaCodeAgainstCurrentUserCommandResult, ErrorData>(
                        new ValidateEmailMfaCodeAgainstCurrentUserCommandResult(TestVariables.UserId)));

            var authenticationService = new Mock<IAuthenticationService>();
            authenticationService.Setup(x => x.SignInUserFromPartialStateAsync(It.IsAny<Guid>()))
                .ReturnsAsync(string.Empty);

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var page = new ValidateEmailMfaCode(mediator.Object, authenticationService.Object,
                currentAuthenticatedUserProvider.Object) { PageModel = new ValidateEmailMfaCode.Model() };

            var result = Assert.IsType<RedirectToPageResult>(await page.OnPost());
            Assert.Equal(PageLocations.AppDashboard, result.PageName);
        }

        [Fact]
        public async Task OnPost_GivenCommandSucceedsWithReturnUrl_ExpectUserToBeAuthenticatedAndRedirectedToUrl()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.Send(
                    It.IsAny<ValidateEmailMfaCodeAgainstCurrentUserCommand>(),
                    It.IsAny<CancellationToken>())).ReturnsAsync(() =>
                    Result.Ok<ValidateEmailMfaCodeAgainstCurrentUserCommandResult, ErrorData>(
                        new ValidateEmailMfaCodeAgainstCurrentUserCommandResult(TestVariables.UserId)));

            var authenticationService = new Mock<IAuthenticationService>();
            authenticationService.Setup(x => x.SignInUserFromPartialStateAsync(It.IsAny<Guid>()))
                .ReturnsAsync("/some-page");

            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var page = new ValidateEmailMfaCode(mediator.Object, authenticationService.Object,
                currentAuthenticatedUserProvider.Object) { PageModel = new ValidateEmailMfaCode.Model() };

            Assert.IsType<LocalRedirectResult>(await page.OnPost());
        }

        [Fact]
        public async Task OnPost_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();
            var authenticationService = new Mock<IAuthenticationService>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var page = new ValidateEmailMfaCode(mediator.Object, authenticationService.Object,
                currentAuthenticatedUserProvider.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPost();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostAppMfaAsync_GivenCommandFails_ExpectPrgErrorStateAndRedirectToSamePage()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AppMfaRequestedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));

            var authenticationService = new Mock<IAuthenticationService>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var page = new ValidateEmailMfaCode(mediator.Object, authenticationService.Object,
                currentAuthenticatedUserProvider.Object);

            Assert.IsType<RedirectToPageResult>(await page.OnPostAppMfaAsync());
            Assert.Equal(PrgState.Failed, page.PrgState);
        }

        [Fact]
        public async Task OnPostAppMfaAsync_GivenCommandSucceeds_ExpectRedirectAppMfaPage()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<AppMfaRequestedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());

            var authenticationService = new Mock<IAuthenticationService>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var page = new ValidateEmailMfaCode(mediator.Object, authenticationService.Object,
                currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<RedirectToPageResult>(await page.OnPostAppMfaAsync());
            Assert.Equal(PageLocations.AuthAppMfa, result.PageName);
        }

        [Fact]
        public async Task OnPostDeviceMfaAsync_GivenCommandFails_ExpectPrgErrorStateAndRedirectToSamePage()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DeviceMfaRequestCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    Result.Fail<DeviceMfaRequestCommandResult, ErrorData>(
                        new ErrorData(ErrorCodes.AuthenticationFailed)));

            var authenticationService = new Mock<IAuthenticationService>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var page = new ValidateEmailMfaCode(mediator.Object, authenticationService.Object,
                currentAuthenticatedUserProvider.Object);

            Assert.IsType<RedirectToPageResult>(await page.OnPostDeviceMfaAsync());
            Assert.Equal(PrgState.Failed, page.PrgState);
        }

        [Fact]
        public async Task OnPostDeviceMfaAsync_GivenCommandSucceeds_ExpectRedirectDeviceMfaPage()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DeviceMfaRequestCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    Result.Ok<DeviceMfaRequestCommandResult, ErrorData>(
                        new DeviceMfaRequestCommandResult(new AssertionOptions())));

            var authenticationService = new Mock<IAuthenticationService>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var page = new ValidateEmailMfaCode(mediator.Object, authenticationService.Object,
                currentAuthenticatedUserProvider.Object)
            {
                TempData = new Mock<ITempDataDictionary>().Object,
            };

            var result = Assert.IsType<RedirectToPageResult>(await page.OnPostDeviceMfaAsync());
            Assert.Equal(PageLocations.AuthDeviceMfa, result.PageName);
        }

        public class ValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var model = new ValidateEmailMfaCode.Model { Code = "code" };
                var validator = new ValidateEmailMfaCode.Validator();
                var result = validator.Validate(model);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenCodeIsEmpty_ExpectValidationFailure()
            {
                var model = new ValidateEmailMfaCode.Model { Code = string.Empty };
                var validator = new ValidateEmailMfaCode.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "Code");
            }

            [Fact]
            public void Validate_GivenCodeIsNull_ExpectValidationFailure()
            {
                var model = new ValidateEmailMfaCode.Model { Code = null };
                var validator = new ValidateEmailMfaCode.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "Code");
            }
        }
    }
}