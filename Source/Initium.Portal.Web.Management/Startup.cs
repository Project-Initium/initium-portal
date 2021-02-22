// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Web.Infrastructure.ServiceConfiguration;
using Initium.Portal.Web.Management.Infrastructure.ServiceConfiguration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace Initium.Portal.Web.Management
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddJsonFile($"appsettings.{Environment.MachineName}.json", true, true);

            builder.AddEnvironmentVariables();

            this.Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddFeatureManagement();
            services
                .AddMemoryCacheSettings()
                .AddCustomizedStaticFiles()
                .AddCustomizedMultiTenant()
                .AddDataStores(this.Configuration)
                .AddConfigurationRoot()
                .AddCustomizedMediatR()
                .AddSettings(this.Configuration)
                .AddHttpContextAccessor()
                .AddCustomizedMvc()
                .AddCustomizedAuthentication(this.Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app
                .UseMultiTenant()
                .AddCustomizedErrorResponse(env)
                .UseXContentTypeOptions()
                .UseReferrerPolicy(opts => opts.NoReferrer())
                .UseCustomizedCsp(env)
                .UseCustomizedStaticFiles()
                .UseRouting()
                .UseXfo(xfo => xfo.Deny())
                .UseRedirectValidation()
                .UseAuthentication()
                .UseAuthorization()
                .UseStackify(env)
                .UseCustomizedEndpoints();
        }
    }
}