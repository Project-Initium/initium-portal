﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using FluentValidation.AspNetCore;
using Initium.Portal.Web.Infrastructure.Extensions;
using Initium.Portal.Web.Infrastructure.Formatters;
using Initium.Portal.Web.Infrastructure.Middleware;
using Initium.Portal.Web.Pages.FirstRun;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using NWebsec.AspNetCore.Mvc.Csp;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class WebConfig
    {
        public static IServiceCollection AddCustomizedMvc(this IServiceCollection services)
        {
            services.AddOData();
            services
                .AddMvc(opts =>
                {
                    opts.Filters.Add(typeof(CspAttribute));
                    opts.Filters.Add(new CspDefaultSrcAttribute { Self = true });
                    opts.InputFormatters.Insert(0, new CspReportBodyFormatter());
                }).AddFluentValidation(fv =>
                {
                    fv.RegisterValidatorsFromAssemblyContaining<InitialUserSetup.Validator>();
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

        public static IApplicationBuilder AddCustomizedErrorResponse(
            this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                app.UseMiddleware<NotFoundMiddleware>();
            }

            return app;
        }

        public static IApplicationBuilder UseCustomizedEndpoints(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            });

            app.UseMvc(routeBuilder =>
            {
                routeBuilder.MapODataServiceRoute("odata", "odata", GetEdmModel());
                routeBuilder.Select().Expand().Filter().OrderBy().Count().MaxTop(int.MaxValue);
            });

            return app;
        }

        private static IEdmModel GetEdmModel()
        {
            var model = new ODataConventionModelBuilder()
                .SetupUserEntity()
                .SetupRoleEntity()
                .SetupUserNotificationEntity()
                .SetupSystemAlertEntity()
                .GetEdmModel();

            return model;
        }
    }
}