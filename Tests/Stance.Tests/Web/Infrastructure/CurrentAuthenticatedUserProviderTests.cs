// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Moq;
using Newtonsoft.Json;
using Stance.Core;
using Stance.Core.Constants;
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
            var unauthenticatedUser = new UnauthenticatedUser(Guid.NewGuid(), MfaProvider.None);
            var httpContextAccessor = new Mock<IHttpContextAccessor>();
            httpContextAccessor.Setup(x => x.HttpContext.User.Identity.IsAuthenticated).Returns(true);
            httpContextAccessor.Setup(x => x.HttpContext.User.HasClaim(It.IsAny<Predicate<Claim>>()))
                .Returns((Predicate<Claim> x) => x.Invoke(new Claim(ClaimTypes.Anonymous, string.Empty)));
            httpContextAccessor.Setup(x => x.HttpContext.User.Claims).Returns(new List<Claim>
            {
                new Claim(ClaimTypes.Anonymous, JsonConvert.SerializeObject(unauthenticatedUser)),
            });

            var currentAuthenticatedUserProvider = new CurrentAuthenticatedUserProvider(httpContextAccessor.Object);
            var maybe = currentAuthenticatedUserProvider.CurrentAuthenticatedUser;

            Assert.True(maybe.HasValue);
            var user = Assert.IsType<UnauthenticatedUser>(maybe.Value);
            Assert.Equal(unauthenticatedUser.UserId, user.UserId);
            Assert.Equal(unauthenticatedUser.SetupMfaProviders, user.SetupMfaProviders);
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
            var user = Assert.IsType<AuthenticatedUser>(maybe.Value);
            Assert.Equal(userProfile.UserId, maybe.Value.UserId);
            Assert.Equal(userProfile.FirstName, user.FirstName);
            Assert.Equal(userProfile.LastName, user.LastName);
            Assert.Equal(userProfile.EmailAddress, user.EmailAddress);
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