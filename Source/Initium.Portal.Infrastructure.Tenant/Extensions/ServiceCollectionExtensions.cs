// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Database;
using Initium.Portal.Infrastructure.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Infrastructure.Tenant.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomizedDataEntityFramework(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddEntityFrameworkSqlServer()
                .AddDbContext<GenericDataContext>();

            serviceCollection
                .AddCoreEntityTypeConfigurationProviders();

            return serviceCollection;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddCoreRepositories();

            return serviceCollection;
        }
    }
}