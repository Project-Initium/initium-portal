// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Infrastructure;
using Initium.Portal.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class DataStoreConfig
    {
        public static IServiceCollection AddDataStores(
            this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddDistributedMemoryCache();
            services.AddEntityFrameworkNpgsql()
                .AddDbContext<DataContext>(options =>
                {
                    options.UseNpgsql(configuration["query:connectionString"]);
                }).AddDbContext<QueryContext>(options =>
                {
                    options.UseNpgsql(configuration["query:connectionString"]);
                });

            return services;
        }
    }
}