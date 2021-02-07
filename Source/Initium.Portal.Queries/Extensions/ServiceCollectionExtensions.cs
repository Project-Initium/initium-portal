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
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, ResourceEntityTypeConfigurationProvider>();
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, RoleEntityTypeConfigurationProvider>();
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, RoleResourceEntityTypeConfigurationProvider>();
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, SystemAlertEntityTypeConfigurationProvider>();
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, UserEntityTypeConfigurationProvider>();
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, UserNotificationEntityTypeConfigurationProvider>();
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, UserRoleEntityTypeConfigurationProvider>();
            return serviceCollection;
        }
    }
}