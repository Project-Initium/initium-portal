// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Web.Infrastructure.ServiceConfiguration;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Web.Management.Infrastructure.ServiceConfiguration
{
    public static class MultiTenantConfig
    {
        public static IServiceCollection AddCustomizedMultiTenant(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddMultiTenant<FeatureBasedTenantInfo>()
                .WithStaticStrategy("default")
                .WithManagementStore();

            return serviceCollection;
        }
    }
}