// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stance.Queries;

namespace Stance.Web.Infrastructure.ServiceConfiguration
{
    public static class SettingsConfig
    {
        public static IServiceCollection AddSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<QuerySettings>(configuration.GetSection("query"));
            return services;
        }
    }
}