// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Internal;
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
            authServiceMock
                .Setup(x => x.SignInAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()))
                .Callback((HttpContext c, string s, ClaimsPrincipal p, AuthenticationProperties a) =>
                {
                    Assert.Contains(p.Claims, x => x.Type == ClaimTypes.Upn && x.Value == userId.ToString());
                    Assert.Contains(p.Claims, x => x.Type == ClaimTypes.Email && x.Value == new string('*', 6));
                })
                .Returns(Task.CompletedTask);
            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(_ => _.GetService(typeof(IAuthenticationService)))
                .Returns(authServiceMock.Object);

            var httpContext = new DefaultHttpContext()
            {
                RequestServices = serviceProviderMock.Object,
            };
            await httpContext.SignInUser(new HttpContextExtensions.UserProfile(userId, new string('*', 6)));

            authServiceMock.Verify(x => x.SignInAsync(It.IsAny<HttpContext>(), CookieAuthenticationDefaults.AuthenticationScheme, It.IsAny<ClaimsPrincipal>(), It.IsAny<AuthenticationProperties>()), Times.Once);
        }
    }
}