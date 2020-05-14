// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Settings;
using Initium.Portal.Queries;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class SettingsConfig
    {
        public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<QuerySettings>(configuration.GetSection("query"));
            services.Configure<SecuritySettings>(configuration.GetSection("Security"));
            return services;
        }
    }
}