// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Security.Claims;
using Initium.Portal.Web.Infrastructure.Extensions;
using Xunit;

namespace Initium.Portal.Tests.Web.Infrastructure.Extensions
{
    public class ClaimsPrincipalExtensionsTests
    {
        [Fact]
        public void HasPermissions_GiveUserHasSystemClaim_ExpectTrue()
        {
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.System, string.Empty),
            }));
            Assert.True(claimsPrincipal.HasPermissions("some-resource"));
        }

        [Fact]
        public void HasPermissions_GiveUserHasResource_ExpectTrue()
        {
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
            {
                new Claim(ClaimTypes.Role, "some-resource"),
            }));
            Assert.True(claimsPrincipal.HasPermissions("some-resource"));
        }

        [Fact]
        public void HasPermissions_GiveUseroesNotHaveResource_ExpectFalse()
        {
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            Assert.False(claimsPrincipal.HasPermissions("some-resource"));
        }
    }
}