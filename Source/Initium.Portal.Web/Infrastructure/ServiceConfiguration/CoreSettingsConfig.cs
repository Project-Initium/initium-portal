﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class CoreSettingsConfig
    {
        public static IServiceCollection AddCoreSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<SecuritySettings>(configuration.GetSection("Security"));
            services.Configure<MultiTenantSettings>(configuration.GetSection("MultiTenant"));
            return services;
        }
    }
}