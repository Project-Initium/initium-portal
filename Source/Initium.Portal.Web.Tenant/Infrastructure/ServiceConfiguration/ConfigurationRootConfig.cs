// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Web.Infrastructure.ServiceConfiguration;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Web.Tenant.Infrastructure.ServiceConfiguration
{
    public static class ConfigurationRootConfig
    {
        public static IServiceCollection AddConfigurationRoot(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddCoreConfigurationRoot();

            return serviceCollection;
        }
    }
}