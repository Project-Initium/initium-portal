// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Database;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.EntityTypeConfigurationProviders;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Queries.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCoreQueryServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IUserQueryService, UserQueryService>();
            serviceCollection.AddScoped<IRoleQueryService, RoleQueryService>();
            serviceCollection.AddScoped<ISystemAlertQueryService, SystemAlertQueryService>();
            serviceCollection.AddScoped<IUserNotificationQueryService, UserNotificationQueryService>();
            serviceCollection.AddScoped<IResourceQueryService, ResourceQueryService>();

            return serviceCollection;
        }

        public static IServiceCollection AddCoreEntityTypeConfigurationProviders(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, ReadResourceEntityTypeConfigurationProvider>();
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, ReadRoleEntityTypeConfigurationProvider>();
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, ReadRoleResourceEntityTypeConfigurationProvider>();
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, ReadSystemAlertEntityTypeConfigurationProvider>();
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, ReadUserEntityTypeConfigurationProvider>();
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, ReadUserNotificationEntityTypeConfigurationProvider>();
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, ReadUserRoleEntityTypeConfigurationProvider>();
            return serviceCollection;
        }
    }
}