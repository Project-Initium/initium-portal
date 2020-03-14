// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using Stance.Web.Infrastructure.Attributes;
using Xunit;

namespace Stance.Tests.Web.Infrastructure.Attributes
{
    public class ResourceBasedAuthorizeAttributeTests
    {
        [Fact]
        public void OnAuthorization_GivenUserIsAuthenticatedAndButHasNoClaim_ExpectContextWithResultOfForbidden()
        {
            var resourceBasedAuthorizeAttribute = new ResourceBasedAuthorizeAttribute(string.Empty);
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.User.Identity.IsAuthenticated).Returns(true);
            var authorizationFilterContext =
                new AuthorizationFilterContext(
                    new ActionContext(httpContext.Object, new RouteData(), new ActionDescriptor()),
                    new List<IFilterMetadata>());

            resourceBasedAuthorizeAttribute.OnAuthorization(authorizationFilterContext);
            var result = Assert.IsType<StatusCodeResult>(authorizationFilterContext.Result);
            Assert.Equal(403, result.StatusCode);
        }

        [Fact]
        public void OnAuthorization_GivenUserIsAuthenticatedAndHasAdminClaimOfForTheGivenResource_ExpectNullResult()
        {
            var resourceBasedAuthorizeAttribute = new ResourceBasedAuthorizeAttribute("some-resource");
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.User.Identity.IsAuthenticated).Returns(true);
            httpContext.Setup(x => x.User.HasClaim(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var authorizationFilterContext =
                new AuthorizationFilterContext(
                    new ActionContext(httpContext.Object, new RouteData(), new ActionDescriptor()),
                    new List<IFilterMetadata>());

            resourceBasedAuthorizeAttribute.OnAuthorization(authorizationFilterContext);
            Assert.Null(authorizationFilterContext.Result);
        }

        [Fact]
        public void OnAuthorization_GivenUserIsAuthenticatedAndHasAdminClaimOfSystem_ExpectNullResult()
        {
            var resourceBasedAuthorizeAttribute = new ResourceBasedAuthorizeAttribute(string.Empty);
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.User.Identity.IsAuthenticated).Returns(true);
            httpContext.Setup(x => x.User.HasClaim(It.IsAny<Predicate<Claim>>()))
                .Returns((Predicate<Claim> x) => x.Invoke(new Claim(ClaimTypes.System, string.Empty)));
            var authorizationFilterContext =
                new AuthorizationFilterContext(
                    new ActionContext(httpContext.Object, new RouteData(), new ActionDescriptor()),
                    new List<IFilterMetadata>());

            resourceBasedAuthorizeAttribute.OnAuthorization(authorizationFilterContext);
            Assert.Null(authorizationFilterContext.Result);
        }

        [Fact]
        public void OnAuthorization_GivenUserIsNotAuthenticated_ExpectContextWithResultOfUnauthorized()
        {
            var resourceBasedAuthorizeAttribute = new ResourceBasedAuthorizeAttribute(string.Empty);
            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(x => x.User.Identity.IsAuthenticated).Returns(false);
            var authorizationFilterContext =
                new AuthorizationFilterContext(
                    new ActionContext(httpContext.Object, new RouteData(), new ActionDescriptor()),
                    new List<IFilterMetadata>());

            resourceBasedAuthorizeAttribute.OnAuthorization(authorizationFilterContext);
            var result = Assert.IsType<StatusCodeResult>(authorizationFilterContext.Result);
            Assert.Equal(401, result.StatusCode);
        }
    }
}