// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Infrastructure.Tenant.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomizedDataEntityFramework(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddEntityFrameworkSqlServer()
                .AddDbContext<TenantDataContext>();

            serviceCollection.AddScoped<ICoreDataContext>(provider => provider.GetRequiredService<TenantDataContext>());
            serviceCollection.AddScoped<ITenantDataContext>(provider => provider.GetRequiredService<TenantDataContext>());

            return serviceCollection;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            return serviceCollection;
        }
    }
}