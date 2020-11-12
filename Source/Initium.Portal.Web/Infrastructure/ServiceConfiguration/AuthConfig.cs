// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class AuthConfig
    {
        public static IServiceCollection AddCustomizedAuthentication(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/auth/sign-in";
                    options.Cookie.Name = $"{configuration["Site:Name"]}.auth";
                })
                .AddCookie("login-partial");

            return services;
        }
    }
}