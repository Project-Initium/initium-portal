// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;

namespace Stance.Web.Infrastructure.Extensions
{
    public static class HttpContextExtensions
    {
        internal static async Task SignInUserAsync(this HttpContext httpContext, UserProfile userProfile)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Upn, userProfile.UserId.ToString()),
                new Claim(ClaimTypes.Email, userProfile.EmailAddress),
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();

            await httpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        internal static async Task SignInUserPartiallyAsync(this HttpContext httpContext, UserProfile userProfile,
            string returnUrl = null)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Upn, userProfile.UserId.ToString()),
                new Claim(ClaimTypes.Email, userProfile.EmailAddress),
            };

            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                claims.Add(new Claim(ClaimTypes.UserData, returnUrl));
            }

            await httpContext.SignInAsync("login-partial", new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)));
        }

        internal static async Task<string> SignInUserFromPartialStateAsync(this HttpContext httpContext, UserProfile userProfile)
        {
            var returnUrl = httpContext.User.HasClaim(x => x.Type == ClaimTypes.UserData)
                ? httpContext.User.Claims.First(x => x.Type == ClaimTypes.UserData).Value
                : string.Empty;
            await httpContext.SignOutAsync("login-partial");

            await httpContext.SignInUserAsync(userProfile);

            return returnUrl;
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