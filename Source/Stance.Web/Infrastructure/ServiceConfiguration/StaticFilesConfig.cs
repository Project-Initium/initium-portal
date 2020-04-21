using Microsoft.AspNetCore.Builder;

namespace Stance.Web.Infrastructure.ServiceConfiguration
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