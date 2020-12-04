// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Reflection;
using FluentValidation.AspNetCore;
using Initium.Portal.Web.Infrastructure.Extensions;
using Initium.Portal.Web.Infrastructure.Formatters;
using Initium.Portal.Web.Infrastructure.Middleware;
using Initium.Portal.Web.Infrastructure.ServiceConfiguration;
using Initium.Portal.Web.Management.Infrastructure.Extensions;
using Initium.Portal.Web.Management.Pages.App.Tenants;
using Initium.Portal.Web.Pages.FirstRun;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OData.Edm;

namespace Initium.Portal.Web.Management.Infrastructure.ServiceConfiguration
{
    public static class WebConfig
    {
        public static IServiceCollection AddCustomizedMvc(this IServiceCollection services)
        {
            services.AddCoreCustomizedMvc(new List<Assembly>
            {
                typeof(CreateTenant.Validator).Assembly,
            });
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
                .SetupTenantEntity()
                .GetEdmModel();

            return model;
        }
    }
}