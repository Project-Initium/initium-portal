// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;

namespace Stance.Web.Infrastructure.ServiceConfiguration
{
    public static class SecurityConfig
    {
        public static IApplicationBuilder UseCustomizedCsp(this IApplicationBuilder app)
        {
            app.UseCsp(options => options
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
                        "sha256-YR1frBL90ajpCi4iwhV84sqi+QuNmvPZPzhUtAnPR3Q=");
                })
                .FontSources(s =>
                {
                    s.Self();
                    s.CustomSources(
                        "https://fonts.gstatic.com",
                        "https://cdnjs.cloudflare.com");
                })
                .ReportUris(r => r.Uris("/api/security/csp-report")));

            return app;
        }
    }
}