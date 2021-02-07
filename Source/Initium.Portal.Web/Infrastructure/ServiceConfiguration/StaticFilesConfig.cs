// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class StaticFilesConfig
    {
        public static IServiceCollection AddCustomizedStaticFiles(this IServiceCollection services)
        {
            return services;
        }

        public static IApplicationBuilder UseCustomizedStaticFiles(this IApplicationBuilder app)
        {
            var mimeTypeProvider = new FileExtensionContentTypeProvider();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = context =>
                {
                    var headers = context.Context.Response.Headers;
                    var contentType = headers["Content-Type"];

                    if (contentType != "application/x-gzip" && !context.File.Name.EndsWith(".gz"))
                    {
                        return;
                    }

                    var fileNameToTry = context.File.Name.Substring(0, context.File.Name.Length - 3);

                    if (mimeTypeProvider.TryGetContentType(fileNameToTry, out var mimeType))
                    {
                        headers.Add("Content-Encoding", "gzip");
                        headers["Content-Type"] = mimeType;
                    }
                },
            });

            return app;
        }
    }
}