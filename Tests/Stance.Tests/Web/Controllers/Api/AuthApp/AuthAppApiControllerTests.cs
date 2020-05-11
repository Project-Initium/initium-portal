// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using ResultMonad;
using Stance.Core;
using Stance.Core.Contracts;
using Stance.Core.Domain;
using Stance.Core.Settings;
using Stance.Domain.CommandResults.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Controllers.Api.AuthApp;
using Stance.Web.Controllers.Api.AuthApp.Models;
using Xunit;

namespace Stance.Tests.Web.Controllers.Api.AuthApp
{
    public class AuthAppApiControllerTests
    {
        [Fact]
        public async Task EnrollAuthApp_GivenExecutionFails_ExpectFailedResult()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EnrollAuthenticatorAppCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            var urlEncoder = new Mock<UrlEncoder>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var controller = new AuthAppApiController(mediator.Object, securitySettings.Object, urlEncoder.Object,
                currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.EnrollAuthApp(new EnrollAuthAppRequest()));
            var response = Assert.IsType<EnrollAuthAppResponse>(result.Value);
            Assert.False(response.IsSuccess);
        }

        [Fact]
        public async Task EnrollAuthApp_GivenExecutionSucceeds_ExpectSuccessfulResult()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EnrollAuthenticatorAppCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ResultWithError.Ok<ErrorData>());
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            var urlEncoder = new Mock<UrlEncoder>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var controller = new AuthAppApiController(mediator.Object, securitySettings.Object, urlEncoder.Object,
                currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.EnrollAuthApp(new EnrollAuthAppRequest()));
            var response = Assert.IsType<EnrollAuthAppResponse>(result.Value);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task EnrollAuthApp_GivenInvalidModelState_ExpectFailedResult()
        {
            var mediator = new Mock<IMediator>();
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            var urlEncoder = new Mock<UrlEncoder>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var controller = new AuthAppApiController(mediator.Object, securitySettings.Object, urlEncoder.Object,
                currentAuthenticatedUserProvider.Object);

            controller.ModelState.AddModelError("key", "error-message");
            var result = Assert.IsType<JsonResult>(await controller.EnrollAuthApp(null));
            var response = Assert.IsType<EnrollAuthAppResponse>(result.Value);
            Assert.False(response.IsSuccess);
        }

        [Fact]
        public async Task InitiateAuthAppEnrollment_GivenExecutionFails_ExpectFailedResult()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x =>
                    x.Send(It.IsAny<InitiateAuthenticatorAppEnrollmentCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                    Result.Fail<InitiateAuthenticatorAppEnrollmentCommandResult, ErrorData>(
                        new ErrorData(ErrorCodes.AuthenticationFailed)));
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            var urlEncoder = new Mock<UrlEncoder>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(
                Maybe.From(
                    new AuthenticatedUser(TestVariables.UserId, "email-address", "first-name", "last-name") as
                        ISystemUser));

            var controller = new AuthAppApiController(mediator.Object, securitySettings.Object, urlEncoder.Object,
                currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.InitiateAuthAppEnrollment());
            var response = Assert.IsType<InitiateAuthAppEnrollmentResponse>(result.Value);
            Assert.False(response.IsSuccess);
        }

        [Fact]
        public async Task InitiateAuthAppEnrollment_GivenExecutionSuccess_ExpectSuccessfulResult()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<InitiateAuthenticatorAppEnrollmentCommand>(), CancellationToken.None))
                .ReturnsAsync(() =>
                    Result.Ok<InitiateAuthenticatorAppEnrollmentCommandResult, ErrorData>(
                        new InitiateAuthenticatorAppEnrollmentCommandResult("some-key")));
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            securitySettings.Setup(x => x.Value).Returns(new SecuritySettings
            {
                SiteName = "site-name",
            });
            var urlEncoder = new Mock<UrlEncoder>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(
                Maybe.From(
                    new AuthenticatedUser(TestVariables.UserId, "email-address", "first-name", "last-name") as
                        ISystemUser));

            var controller = new AuthAppApiController(mediator.Object, securitySettings.Object, urlEncoder.Object,
                currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.InitiateAuthAppEnrollment());
            var response = Assert.IsType<InitiateAuthAppEnrollmentResponse>(result.Value);
            Assert.True(response.IsSuccess);
            Assert.Equal("some-key", response.SharedKey);
            Assert.Equal("otpauth://totp/:?secret=some-key&issuer=&digits=6", response.AuthenticatorUri);
            Assert.Equal("some -key", response.FormattedSharedKey);
        }

        [Fact]
        public async Task InitiateAuthAppEnrollment_GivenNoUserIsAuthenticated_ExpectFailedResult()
        {
            var mediator = new Mock<IMediator>();
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            var urlEncoder = new Mock<UrlEncoder>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);

            var controller = new AuthAppApiController(mediator.Object, securitySettings.Object, urlEncoder.Object,
                currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.InitiateAuthAppEnrollment());
            var response = Assert.IsType<InitiateAuthAppEnrollmentResponse>(result.Value);
            Assert.False(response.IsSuccess);
        }

        [Fact]
        public async Task InitiateAuthAppEnrollment_GivenUserIsNotAuthenticatedUser_ExpectFailedResult()
        {
            var mediator = new Mock<IMediator>();
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            var urlEncoder = new Mock<UrlEncoder>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new Mock<ISystemUser>().Object));

            var controller = new AuthAppApiController(mediator.Object, securitySettings.Object, urlEncoder.Object,
                currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.InitiateAuthAppEnrollment());
            var response = Assert.IsType<InitiateAuthAppEnrollmentResponse>(result.Value);
            Assert.False(response.IsSuccess);
        }

        [Fact]
        public async Task RevokeAuthApp_GivenExecutionFails_ExpectFailedResult()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<RevokeAuthenticatorAppCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            var urlEncoder = new Mock<UrlEncoder>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var controller = new AuthAppApiController(mediator.Object, securitySettings.Object, urlEncoder.Object,
                currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.RevokeAuthApp(new RevokeAuthAppRequest()));
            var response = Assert.IsType<RevokeAuthAppResponse>(result.Value);
            Assert.False(response.IsSuccess);
        }

        [Fact]
        public async Task RevokeAuthApp_GivenExecutionSucceeds_ExpectSuccessfulResult()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<RevokeAuthenticatorAppCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            var urlEncoder = new Mock<UrlEncoder>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var controller = new AuthAppApiController(mediator.Object, securitySettings.Object, urlEncoder.Object,
                currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.RevokeAuthApp(new RevokeAuthAppRequest()));
            var response = Assert.IsType<RevokeAuthAppResponse>(result.Value);
            Assert.True(response.IsSuccess);
        }

        [Fact]
        public async Task RevokeAuthApp_GivenInvalidModelState_ExpectFailedResult()
        {
            var mediator = new Mock<IMediator>();
            var securitySettings = new Mock<IOptions<SecuritySettings>>();
            var urlEncoder = new Mock<UrlEncoder>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var controller = new AuthAppApiController(mediator.Object, securitySettings.Object, urlEncoder.Object,
                currentAuthenticatedUserProvider.Object);

            controller.ModelState.AddModelError("key", "error-message");
            var result = Assert.IsType<JsonResult>(await controller.RevokeAuthApp(null));
            var response = Assert.IsType<RevokeAuthAppResponse>(result.Value);
            Assert.False(response.IsSuccess);
        }
    }
}