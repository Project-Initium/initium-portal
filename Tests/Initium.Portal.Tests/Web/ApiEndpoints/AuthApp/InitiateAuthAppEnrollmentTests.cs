// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Initium.Portal.Core;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Settings;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.ApiEndpoints.AuthApp;
using MaybeMonad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.ApiEndpoints.AuthApp
{
    public class InitiateAuthAppEnrollmentTests
    {
        [Fact]
        public async Task HandleAsync_GivenExecutionFails_ExpectFailedResult()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x =>
                    x.Send(It.IsAny<InitiateAuthenticatorAppEnrollmentCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                    Result.Fail<InitiateAuthenticatorAppEnrollmentCommandResult, ErrorData>(
                        new ErrorData(ErrorCodes.AuthenticationFailed)));

            var urlEncoder = new Mock<UrlEncoder>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(
                Maybe.From(
                    new AuthenticatedUser(TestVariables.UserId, "email-address", "first-name", "last-name") as
                        ISystemUser));
            var tenantInfo = new Mock<ITenantInfo>();

            var endpoint = new InitiateAuthAppEnrollment(currentAuthenticatedUserProvider.Object, mediator.Object, urlEncoder.Object, tenantInfo.Object);

            var response = await endpoint.HandleAsync();
            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<InitiateAuthAppEnrollment.EndpointResponse>(rawResult.Value);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task HandleAsync_GivenExecutionSuccess_ExpectSuccessfulResult()
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

            var tenantInfo = new Mock<ITenantInfo>();

            var endpoint = new InitiateAuthAppEnrollment(currentAuthenticatedUserProvider.Object, mediator.Object, urlEncoder.Object, tenantInfo.Object);

            var response = await endpoint.HandleAsync();
            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<InitiateAuthAppEnrollment.EndpointResponse>(rawResult.Value);
            Assert.True(result.IsSuccess);
            Assert.Equal("some-key", result.SharedKey);
            Assert.Equal("otpauth://totp/:?secret=some-key&issuer=&digits=6", result.AuthenticatorUri);
            Assert.Equal("some -key", result.FormattedSharedKey);
        }

        [Fact]
        public async Task HandleAsync_GivenNoUserIsAuthenticated_ExpectFailedResult()
        {
            var mediator = new Mock<IMediator>();
            var urlEncoder = new Mock<UrlEncoder>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);

            var tenantInfo = new Mock<ITenantInfo>();

            var endpoint = new InitiateAuthAppEnrollment(currentAuthenticatedUserProvider.Object, mediator.Object, urlEncoder.Object, tenantInfo.Object);

            var response = await endpoint.HandleAsync();
            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<InitiateAuthAppEnrollment.EndpointResponse>(rawResult.Value);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task HandleAsync_GivenUserIsNotAuthenticatedUser_ExpectFailedResult()
        {
            var mediator = new Mock<IMediator>();
            var urlEncoder = new Mock<UrlEncoder>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new Mock<ISystemUser>().Object));

            var tenantInfo = new Mock<ITenantInfo>();

            var endpoint = new InitiateAuthAppEnrollment(currentAuthenticatedUserProvider.Object, mediator.Object, urlEncoder.Object, tenantInfo.Object);

            var response = await endpoint.HandleAsync();
            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<InitiateAuthAppEnrollment.EndpointResponse>(rawResult.Value);
            Assert.False(result.IsSuccess);
        }
    }
}