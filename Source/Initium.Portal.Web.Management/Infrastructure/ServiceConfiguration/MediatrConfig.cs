// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Reflection;
using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Initium.Portal.Web.Infrastructure.ServiceConfiguration;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Web.Management.Infrastructure.ServiceConfiguration
{
    public static class MediatrConfig
    {
        public static IServiceCollection AddCustomizedMediatR(
            this IServiceCollection serviceCollection)
        {
            var assembly = typeof(CreateTenantCommand).GetTypeInfo().Assembly;
            serviceCollection.AddCoreCustomizedMediatR(new[] { assembly });
            return serviceCollection;
        }
    }
}