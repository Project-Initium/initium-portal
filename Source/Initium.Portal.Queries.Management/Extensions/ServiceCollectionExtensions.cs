// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Database;
using Initium.Portal.Queries.Extensions;
using Initium.Portal.Queries.Management.Contracts;
using Initium.Portal.Queries.Management.EntityTypeConfigurationProviders;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Queries.Management.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddQueryServices(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<ITenantQueryService, TenantQueryService>();

            serviceCollection
                .AddCoreEntityTypeConfigurationProviders()
                .AddScoped<IEntityTypeConfigurationProvider, ReadTenantEntityTypeConfigurationProvider>();

            return serviceCollection;
        }
    }
}