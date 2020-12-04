using System.Collections.Generic;
using System.Reflection;
using FluentValidation.AspNetCore;
using Initium.Portal.Web.Infrastructure.Formatters;
using Initium.Portal.Web.Pages.FirstRun;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using NWebsec.AspNetCore.Mvc.Csp;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class CoreWebConfig
    {
        public static IServiceCollection AddCoreCustomizedMvc(this IServiceCollection services, List<Assembly> assemblies = null)
        {
            assemblies ??= new List<Assembly>();
            assemblies.Add(typeof(InitialUserSetup.Validator).Assembly);

            services.AddOData();
            services
                .AddMvc(opts =>
                {
                    opts.Filters.Add(typeof(CspAttribute));
                    opts.Filters.Add(new CspDefaultSrcAttribute { Self = true });
                    opts.InputFormatters.Insert(0, new CspReportBodyFormatter());
                }).AddFluentValidation(fv =>
                {
                    fv.RegisterValidatorsFromAssemblies(assemblies);
                    fv.ImplicitlyValidateChildProperties = true;
                })
                .AddRazorOptions(options =>
                {
                    options.ViewLocationFormats.Add("/{0}.cshtml");
                    options.PageViewLocationFormats.Add("/{0}.cshtml");
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddNewtonsoftJson();

            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddControllers(mvc => { mvc.EnableEndpointRouting = false; });
            return services;
        }
    }
}