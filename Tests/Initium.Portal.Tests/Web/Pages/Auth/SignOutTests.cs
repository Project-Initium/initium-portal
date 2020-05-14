// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Initium.Portal.Web.Pages.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.Auth
{
    public class SignOutTests
    {
        [Fact]
        public async Task OnGetAsync_GivenRequestIsMade_ExpectUserSignOut()
        {
            var authServiceMock = new Mock<IAuthenticationService>();

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

            var page = new SignOut
            {
                PageContext = pageContext,
            };

            await page.OnGetAsync();

            authServiceMock.Verify(
                x => x.SignOutAsync(It.IsAny<HttpContext>(), It.IsAny<string>(), It.IsAny<AuthenticationProperties>()),
                Times.Once);
        }
    }
}