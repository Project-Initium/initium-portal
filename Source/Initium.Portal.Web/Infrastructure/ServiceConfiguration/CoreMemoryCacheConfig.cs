// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Caching;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class CoreMemoryCacheConfig
    {
        public static IServiceCollection AddMemoryCacheSettings(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddScoped<IDataSerializer, ProtoBufDataSerializer>();
            services.AddScoped<ICustomDistributedCache, CustomDistributedCache>();
            return services;
        }
    }
}