// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Queries.Tenant.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomizedQueryEntityFramework(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddEntityFrameworkSqlServer()
                .AddDbContext<TenantQueryContext>();

            serviceCollection.AddScoped<ICoreQueryContext>(provider => provider.GetRequiredService<TenantQueryContext>());
            serviceCollection.AddScoped<ITenantQueryContext>(provider => provider.GetRequiredService<TenantQueryContext>());

            return serviceCollection;
        }

        public static IServiceCollection AddQueryServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}