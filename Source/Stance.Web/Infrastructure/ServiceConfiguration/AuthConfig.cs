// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;

namespace Stance.Web.Infrastructure.ServiceConfiguration
{
    public static class AuthConfig
    {
        public static IServiceCollection AddCustomizedAuthentication(
            this IServiceCollection services)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/auth/sign-in";
                    options.Cookie.Name = "stance.auth";
                })
                .AddCookie("login-partial");

            return services;
        }
    }
}