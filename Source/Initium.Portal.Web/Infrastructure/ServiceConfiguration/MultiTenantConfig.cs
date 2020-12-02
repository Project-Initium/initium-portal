// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Finbuckle.MultiTenant;
using Initium.Portal.Core.Settings;
using Initium.Portal.Web.Infrastructure.MultiTenant;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaTime;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class MultiTenantConfig
    {
        public static IServiceCollection AddCustomizedMultiTenant(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMultiTenant<TenantInfo>()
                .WithHostStrategy()
                .WithStaticStrategy("default")
                .WithStore(ServiceLifetime.Singleton, sp =>
                {
                    var serviceProvider = sp.CreateScope().ServiceProvider;
                    var multiTenantSettings = serviceProvider.GetRequiredService<IOptions<MultiTenantSettings>>();
                    var distributedCache = serviceProvider.GetRequiredService<IDistributedCache>();
                    var clock = serviceProvider.GetRequiredService<IClock>();
                    return new CustomMultiTenantStore(multiTenantSettings, distributedCache, clock);
                });

            return serviceCollection;
        }
    }
}