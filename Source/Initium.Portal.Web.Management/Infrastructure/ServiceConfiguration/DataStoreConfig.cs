// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Infrastructure.Extensions;
using Initium.Portal.Infrastructure.Management.Extensions;
using Initium.Portal.Queries.Extensions;
using Initium.Portal.Queries.Management.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Web.Management.Infrastructure.ServiceConfiguration
{
    public static class DataStoreConfig
    {
        public static IServiceCollection AddDataStores(
            this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddDistributedMemoryCache();

            services.AddRepositories();

            services.AddCoreQueryServices();
            services.AddQueryServices();

            services.AddCustomizedDataEntityFramework();

            return services;
        }
    }
}