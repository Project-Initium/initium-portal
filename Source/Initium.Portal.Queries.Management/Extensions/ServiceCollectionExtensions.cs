// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Queries.Management.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Queries.Management.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomizedQueryEntityFramework(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddEntityFrameworkSqlServer()
                .AddDbContext<ManagementQueryContext>();

            serviceCollection.AddScoped<ICoreQueryContext>(provider => provider.GetRequiredService<ManagementQueryContext>());
            serviceCollection.AddScoped<IManagementQueryContext>(provider => provider.GetRequiredService<ManagementQueryContext>());

            return serviceCollection;
        }

        public static IServiceCollection AddQueryServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITenantQueryService, TenantQueryService>();

            return serviceCollection;
        }
    }
}