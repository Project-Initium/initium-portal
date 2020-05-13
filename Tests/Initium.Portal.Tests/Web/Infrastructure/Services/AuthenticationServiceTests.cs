// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Initium.Portal.Core.Constants;
using Initium.Portal.Queries.Contracts.Static;
using Initium.Portal.Queries.Static.Models.User;
using MaybeMonad;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;
using AuthenticationService = Initium.Portal.Web.Infrastructure.Services.AuthenticationService;

namespace Initium.Portal.Tests.Web.Infrastructure.Services
{
    public class AuthenticationServiceTests
    {
        [Fact]
        public async Task SignInUserAsync_GivenUserDoesNotExist_ExpectException()
        {
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetSystemProfileByUserId(It.IsAny<Guid>()))
                .ReturnsAsync(() => Maybe<SystemProfileModel>.Nothing);

            var authenticationService =
                new AuthenticationService(
                    httpContextAccessor.Object, userQueries.Object);

            await Assert.ThrowsAsync<InvalidOperationException>(() => authenticationService.SignInUserAsync(Guid.Empty));
        }

        [Fact]
        public async Task SignInUserAsync_GivenUserExists_ExpectIdentityToBeSet()
        {
            var userId = Guid.NewGuid();
            var authServiceMock = new Mock<IAuthenticationService>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object,
            };

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var userQueries = new Mock<IUserQueries>();
            userQueries.Setup(x => x.GetSystemProfileByUserId(It.IsAny<Guid>()))
                .ReturnsAsync(
                    () => Maybe.From(new SystemProfileModel(
                        new string('*', 5),
                        new string('*', 5),
                        new string('*', 5),
                        true,
                        new List<string>())));
            var authenticationService =
                new AuthenticationService(
                    httpContextAccessor.Object, userQueries.Object);

            await authenticationService.SignInUserAsync(userId);

            authServiceMock.Verify(
                x => x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    It.Is<ClaimsPrincipal>(x =>
                        x.HasClaim(c => c.Type == ClaimTypes.Upn && c.Value == userId.ToString()) &&
                        x.HasClaim(c => c.Type == ClaimTypes.UserData)),
                    It.IsAny<AuthenticationProperties>()), Times.Once);
        }

        [Fact]
        public async Task SignInUserPartiallyAsync_GivenValidUserProfileAndNoReturnUrl_ExpectIdentityToBeSetWithPartialSchemaAndNoUserData()
        {
            var userId = Guid.NewGuid();
            var authServiceMock = new Mock<IAuthenticationService>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object,
            };

            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var userQueries = new Mock<IUserQueries>();

            var authenticationService =
                new AuthenticationService(
                    httpContextAccessor.Object, userQueries.Object);
            await authenticationService.SignInUserPartiallyAsync(userId, MfaProvider.App);

            authServiceMock.Verify(
                x => x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    "login-partial",
                    It.Is<ClaimsPrincipal>(x =>
                        x.HasClaim(c => c.Type == ClaimTypes.Anonymous) &&
                        !x.HasClaim(c => c.Type == ClaimTypes.UserData)),
                    It.IsAny<AuthenticationProperties>()), Times.Once);
        }

        [Fact]
        public async Task SignInUserPartiallyAsync_GivenValidUserProfileAndReturnUrl_ExpectIdentityToBeSetWithPartialSchemaAndUserDataSet()
        {
            var userId = Guid.NewGuid();
            var authServiceMock = new Mock<IAuthenticationService>();
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);
            var httpContext = new DefaultHttpContext
            {
                RequestServices = serviceProviderMock.Object,
            };
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);
            var userQueries = new Mock<IUserQueries>();

            var authenticationService =
                new AuthenticationService(
                    httpContextAccessor.Object, userQueries.Object);
            await authenticationService.SignInUserPartiallyAsync(userId, MfaProvider.App, new string('*', 6));

            authServiceMock.Verify(
                x => x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    "login-partial",
                    It.Is<ClaimsPrincipal>(x =>
                        x.HasClaim(c => c.Type == ClaimTypes.Anonymous) &&
                        x.HasClaim(c => c.Type == ClaimTypes.UserData)),
                    It.IsAny<AuthenticationProperties>()), Times.Once);
        }
    }
}