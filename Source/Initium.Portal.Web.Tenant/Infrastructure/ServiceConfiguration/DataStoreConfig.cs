// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Infrastructure.Extensions;
using Initium.Portal.Infrastructure.Tenant.Extensions;
using Initium.Portal.Queries.Extensions;
using Initium.Portal.Queries.Tenant.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Web.Tenant.Infrastructure.ServiceConfiguration
{
    public static class DataStoreConfig
    {
        public static IServiceCollection AddDataStores(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();

            services.AddCoreRepositories();
            services.AddRepositories();

            services.AddCoreQueryServices();
            services.AddQueryServices();

            services.AddCustomizedDataEntityFramework();
            services.AddCustomizedQueryEntityFramework();

            return services;
        }
    }
}