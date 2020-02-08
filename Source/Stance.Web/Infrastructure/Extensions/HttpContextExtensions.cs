// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Stance.Web.Infrastructure.Extensions
{
    public static class HttpContextExtensions
    {
        internal static async Task SignInUser(this HttpContext httpContext, UserProfile userProfile)
        {
            var realClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Upn, userProfile.UserId.ToString()),
                new Claim(ClaimTypes.Email, userProfile.EmailAddress),
            };

            var claimsIdentity = new ClaimsIdentity(
                realClaims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        internal class UserProfile
        {
            public UserProfile(Guid userId, string emailAddress)
            {
                this.UserId = userId;
                this.EmailAddress = emailAddress;
            }

            public Guid UserId { get; }

            public string EmailAddress { get; }
        }
    }
}