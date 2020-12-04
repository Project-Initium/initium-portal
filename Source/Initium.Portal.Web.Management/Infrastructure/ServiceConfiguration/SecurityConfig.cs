// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq;
using Initium.Portal.Web.Infrastructure.ServiceConfiguration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using NWebsec.Core.Common.Middleware.Options;

namespace Initium.Portal.Web.Management.Infrastructure.ServiceConfiguration
{
    public static class SecurityConfig
    {
        public static IApplicationBuilder UseCustomizedCsp(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCoreCustomizedCsp(env, ConfigureCspOptions);
            return app;
        }

        private static void ConfigureCspOptions(IFluentCspOptions options)
        {
            options
                .DefaultSources(s => s.Self())
                .ImageSources(s =>
                {
                    s.Self();
                    s.CustomSources(CoreSecurityConfig.CoreCustomImageSources);
                })
                .ScriptSources(s => s.Self())
                .StyleSources(s =>
                {
                    s.Self();
                    s.CustomSources(
                        CoreSecurityConfig.CoreCustomStyleSources
                            .Append("sha256-47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU=")
                            .Append("sha256-YR1frBL90ajpCi4iwhV84sqi+QuNmvPZPzhUtAnPR3Q=")
                            .Append("sha256-KpQHAI/AubL4JrO3VYskOgqSm+Z9nzrIqWB1dTOfCK4=")
                            .Append("sha256-QNbeS1u8jY1P0nlYj8zkA3FloKmdLnW9LuXnqZSW5cU=")
                            .Append("sha256-QNbeS1u8jY1P0nlYj8zkA3FloKmdLnW9LuXnqZSW5cU=").ToArray());
                })
                .FontSources(s =>
                {
                    s.Self();
                    s.CustomSources(CoreSecurityConfig.CoreCustomFontSources);
                })
                .ReportUris(r => r.Uris("/api/security/csp-report"));
        }
    }
}