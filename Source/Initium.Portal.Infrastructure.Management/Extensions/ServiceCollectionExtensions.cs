// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Common.Domain.AggregatesModel.TenantAggregate;
using Initium.Portal.Core.Database;
using Initium.Portal.Infrastructure.Extensions;
using Initium.Portal.Infrastructure.Management.EntityTypeConfigurationProviders;
using Initium.Portal.Infrastructure.Management.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Infrastructure.Management.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCustomizedDataEntityFramework(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddEntityFrameworkSqlServer()
                .AddDbContext<GenericDataContext>();

            serviceCollection
                .AddCoreEntityTypeConfigurationProviders()
                .AddScoped<IEntityTypeConfigurationProvider, TenantEntityTypeConfigurationProvider>();

            return serviceCollection;
        }

        public static IServiceCollection AddRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddCoreRepositories()
                .AddScoped<ITenantRepository, TenantRepository>();
            return serviceCollection;
        }
    }
}