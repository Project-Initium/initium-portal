// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class StaticFilesConfig
    {
        public static IApplicationBuilder UseCustomizedStaticFiles(this IApplicationBuilder app)
        {
            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = content =>
                {
                    if (!content.File.Name.EndsWith(".js.gz"))
                    {
                        return;
                    }

                    content.Context.Response.Headers["Content-Type"] = "text/javascript";
                    content.Context.Response.Headers["Content-Encoding"] = "gzip";
                },
            });

            return app;
        }
    }
}