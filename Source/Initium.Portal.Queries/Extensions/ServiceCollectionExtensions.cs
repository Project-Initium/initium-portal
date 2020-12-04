// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Queries.Contracts;
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
    }
}