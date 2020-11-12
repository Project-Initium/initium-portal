// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NWebsec.Core.Common.Middleware.Options;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class CoreSecurityConfig
    {
        public static readonly string[] CoreCustomImageSources =
        {
            "https://www.gravatar.com",
        };

        public static readonly string[] CoreCustomStyleSources =
        {
            "https://fonts.googleapis.com",
            "https://cdnjs.cloudflare.com",
        };

        public static readonly string[] CoreCustomFontSources =
        {
            "https://fonts.gstatic.com",
            "https://cdnjs.cloudflare.com",
        };

        public static IApplicationBuilder UseCoreCustomizedCsp(
            this IApplicationBuilder app,
            IWebHostEnvironment env,
            Action<IFluentCspOptions> optionsBuilder)
        {
            if (env.IsDevelopment())
            {
                app.UseCspReportOnly(optionsBuilder);
            }
            else
            {
                app.UseCsp(optionsBuilder);
            }

            return app;
        }
    }
}