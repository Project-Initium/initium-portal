// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Moq;
using Stance.Web.Infrastructure.Extensions;
using Xunit;

namespace Stance.Tests.Web.Infrastructure.Extensions
{
    public class HttpContextExtensionsTests
    {
        [Fact]
        public async Task SignInUser_GivenValidUserProfile_ExpectIdentityToBeSet()
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
            await httpContext.SignInUserAsync(new HttpContextExtensions.UserProfile(userId, new string('*', 6), new string('*', 7), new string('*', 8)));

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
            await httpContext.SignInUserPartiallyAsync(userId);

            authServiceMock.Verify(
                x => x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    "login-partial",
                    It.Is<ClaimsPrincipal>(x =>
                        x.HasClaim(c => c.Type == ClaimTypes.Anonymous && c.Value == userId.ToString()) &&
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
            await httpContext.SignInUserPartiallyAsync(userId, new string('*', 6));

            authServiceMock.Verify(
                x => x.SignInAsync(
                    It.IsAny<HttpContext>(),
                    "login-partial",
                    It.Is<ClaimsPrincipal>(x =>
                        x.HasClaim(c => c.Type == ClaimTypes.Anonymous && c.Value == userId.ToString()) &&
                        x.HasClaim(c => c.Type == ClaimTypes.UserData)),
                    It.IsAny<AuthenticationProperties>()), Times.Once);
        }
    }
}