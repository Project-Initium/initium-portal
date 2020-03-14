// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using Stance.Web.Infrastructure;
using Stance.Web.Infrastructure.Services;
using Xunit;

namespace Stance.Tests.Web.Infrastructure
{
    public class CurrentAuthenticatedUserProviderTests
    {
        [Fact]
        public void CurrentAuthenticatedUser_GivenNoUserIsAuthenticated_ExpectMaybeWithNoData()
        {
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(false);

            var currentAuthenticatedUserProvider = new CurrentAuthenticatedUserProvider(httpContextAccessor.Object);
            var maybe = currentAuthenticatedUserProvider.CurrentAuthenticatedUser;

            Assert.True(maybe.HasNoValue);
        }

        [Fact]
        public void CurrentAuthenticatedUser_GivenUserHasAnonymousClaim_ExpectMaybeWithDataHaveIdPropertySet()
        {
            var userId = Guid.NewGuid();
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            httpContextAccessor.Setup(x => x.HttpContext.User.HasClaim(It.IsAny<Predicate<Claim>>()))
                .Returns((Predicate<Claim> x) => x.Invoke(new Claim(ClaimTypes.Anonymous, string.Empty)));
            httpContextAccessor.Setup(x => x.HttpContext.User.Claims).Returns(new List<Claim>
            {
                new Claim(ClaimTypes.Anonymous, userId.ToString()),
            });

            var currentAuthenticatedUserProvider = new CurrentAuthenticatedUserProvider(httpContextAccessor.Object);
            var maybe = currentAuthenticatedUserProvider.CurrentAuthenticatedUser;

            Assert.True(maybe.HasValue);
            Assert.Equal(userId, maybe.Value.UserId);
        }

        [Fact]
        public void CurrentAuthenticatedUser_GivenUserHasUpnClaim_ExpectMaybeWithDataHaveAllPropertySet()
        {
            var userProfile = new AuthenticationService.UserProfile(Guid.NewGuid(), "email-address", "first-name",
                "last-name", false, new List<string>());
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            httpContextAccessor.Setup(x => x.HttpContext.User.HasClaim(It.IsAny<Predicate<Claim>>()))
                .Returns((Predicate<Claim> x) => x.Invoke(new Claim(ClaimTypes.Upn, string.Empty)));
            httpContextAccessor.Setup(x => x.HttpContext.User.Claims).Returns(new List<Claim>
            {
                new Claim(ClaimTypes.Upn, userProfile.UserId.ToString()),
                new Claim(ClaimTypes.UserData, JsonConvert.SerializeObject(userProfile)),
            });

            var currentAuthenticatedUserProvider = new CurrentAuthenticatedUserProvider(httpContextAccessor.Object);
            var maybe = currentAuthenticatedUserProvider.CurrentAuthenticatedUser;

            Assert.True(maybe.HasValue);
            Assert.Equal(userProfile.UserId, maybe.Value.UserId);
            Assert.Equal(userProfile.FirstName, maybe.Value.FirstName);
            Assert.Equal(userProfile.LastName, maybe.Value.LastName);
            Assert.Equal(userProfile.EmailAddress, maybe.Value.EmailAddress);
        }

        [Fact]
        public void CurrentAuthenticatedUser_GivenUserNoClaims_ExpectMaybeWithNoData()
        {
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);

            var currentAuthenticatedUserProvider = new CurrentAuthenticatedUserProvider(httpContextAccessor.Object);
            var maybe = currentAuthenticatedUserProvider.CurrentAuthenticatedUser;

            Assert.True(maybe.HasNoValue);
        }
    }
}