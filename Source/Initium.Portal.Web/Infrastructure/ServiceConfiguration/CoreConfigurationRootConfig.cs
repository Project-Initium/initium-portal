// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Fido2NetLib;
using Finbuckle.MultiTenant;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Web.Infrastructure.Contracts;
using Initium.Portal.Web.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class CoreConfigurationRootConfig
    {
        public static IServiceCollection AddCoreConfigurationRoot(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IClock>(SystemClock.Instance);

            serviceCollection.AddScoped<ICurrentAuthenticatedUserProvider, CurrentAuthenticatedUserProvider>();
            serviceCollection.AddScoped<IAuthenticationService, AuthenticationService>();

            serviceCollection.AddSingleton<IFido2>(ConfigureFido);

            return serviceCollection;
        }

        private static IFido2 ConfigureFido(IServiceProvider arg)
        {
            var httpContextAccessor = arg.GetRequiredService<IHttpContextAccessor>();
            var tenantInfo = httpContextAccessor.HttpContext.GetMultiTenantContext<FeatureBasedTenantInfo>();

            return new Fido2(new Fido2Configuration
            {
                ServerDomain = httpContextAccessor.HttpContext.Request.Host.Host,
                ServerName = tenantInfo.TenantInfo.Name,
                Origin = $"https://{httpContextAccessor.HttpContext.Request.Host.ToString()}",
            });
        }
    }
}