// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stance.Web.Infrastructure.ServiceConfiguration;

namespace Stance.Web
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
            services
                .AddDataStores(this.Configuration)
                .AddConfigurationRoot()
                .AddCustomizedMediatR()
                .AddSettings(this.Configuration)
                .AddCustomizedMvc()
                .AddCustomizedAuthentication();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app
                .AddCustomizedErrorResponse(env)
                .UseXContentTypeOptions()
                .UseReferrerPolicy(opts => opts.NoReferrer())
                .UseCsp(options => options
                    .DefaultSources(s => s.Self())
                    .ImageSources(s =>
                    {
                        s.Self();
                        s.CustomSources = new List<string>
                        {
                            "https://www.gravatar.com",
                        };
                    })
                    .ScriptSources(s => s.Self())
                    .ReportUris(r => r.Uris("/report")))
                .UseCspReportOnly(options => options
                    .DefaultSources(s => s.Self())
                    .ImageSources(s => s.None()))
                .UseStaticFiles()
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