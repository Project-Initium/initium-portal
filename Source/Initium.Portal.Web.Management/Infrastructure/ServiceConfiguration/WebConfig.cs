// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentValidation.AspNetCore;
using Initium.Portal.Web.Infrastructure.Contracts;
using Initium.Portal.Web.Infrastructure.Extensions;
using Initium.Portal.Web.Infrastructure.Formatters;
using Initium.Portal.Web.Infrastructure.Middleware;
using Initium.Portal.Web.Infrastructure.ServiceConfiguration;
using Initium.Portal.Web.Management.Infrastructure.Extensions;
using Initium.Portal.Web.Management.Pages.App.Tenants;
using Initium.Portal.Web.Pages.FirstRun;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace Initium.Portal.Web.Management.Infrastructure.ServiceConfiguration
{
    public static class WebConfig
    {
        public static IServiceCollection AddCustomizedMvc(this IServiceCollection services)
        {
            services.AddCoreCustomizedMvc(
                new List<Assembly>
                {
                    typeof(CreateTenant.Validator).Assembly,
                },
                GetEdmModel());
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
                endpoints.MapControllers();
            });

            // app.UseMvc(routeBuilder =>
            // {
            //     routeBuilder.MapODataServiceRoute("odata", "odata", GetEdmModel());
            //     routeBuilder.Select().Expand().Filter().OrderBy().Count().MaxTop(int.MaxValue);
            // });

            return app;
        }

        private static IEdmModel GetEdmModel()
        {

            var builder = new ODataConventionModelBuilder();
            
            var types = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IODataEntityBuilder).IsAssignableFrom(p) && !p.IsInterface)
                .ToList();
            
            foreach (var instance in types.Select(Activator.CreateInstance))
            {
                if (instance is IODataEntityBuilder odataEntityBuilder)
                {
                    odataEntityBuilder.Configure(builder);
                }
            }
            
            
            
                builder.SetupTenantEntity();

            return builder.GetEdmModel();
        }
    }
}