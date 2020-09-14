// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NWebsec.Core.Common.Middleware.Options;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class SecurityConfig
    {
        public static IApplicationBuilder UseCustomizedCsp(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseCspReportOnly(ConfigureCspOptions);
            }
            else
            {
                app.UseCsp(ConfigureCspOptions);
            }

            return app;
        }

        private static void ConfigureCspOptions(IFluentCspOptions options)
        {
            options
                .DefaultSources(s => s.Self())
                .ImageSources(s =>
                {
                    s.Self();
                    s.CustomSources("https://www.gravatar.com");
                })
                .ScriptSources(s => s.Self())
                .StyleSources(s =>
                {
                    s.Self();
                    s.CustomSources(
                        "https://fonts.googleapis.com",
                        "https://cdnjs.cloudflare.com",
                        "sha256-47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU=",
                        "sha256-YR1frBL90ajpCi4iwhV84sqi+QuNmvPZPzhUtAnPR3Q=",
                        "sha256-KpQHAI/AubL4JrO3VYskOgqSm+Z9nzrIqWB1dTOfCK4=",
                        "sha256-QNbeS1u8jY1P0nlYj8zkA3FloKmdLnW9LuXnqZSW5cU=",
                        "sha256-QNbeS1u8jY1P0nlYj8zkA3FloKmdLnW9LuXnqZSW5cU=");
                })
                .FontSources(s =>
                {
                    s.Self();
                    s.CustomSources(
                        "https://fonts.gstatic.com",
                        "https://cdnjs.cloudflare.com");
                })
                .ReportUris(r => r.Uris("/api/security/csp-report"));
        }
    }
}