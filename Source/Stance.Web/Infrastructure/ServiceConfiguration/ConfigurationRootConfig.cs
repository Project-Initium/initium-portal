// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Fido2NetLib;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaTime;
using Stance.Core.Contracts;
using Stance.Core.Settings;
using Stance.Domain.AggregatesModel.RoleAggregate;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Infrastructure.Repositories;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static;
using Stance.Web.Infrastructure.Contracts;
using Stance.Web.Infrastructure.Services;

namespace Stance.Web.Infrastructure.ServiceConfiguration
{
    public static class ConfigurationRootConfig
    {
        public static IServiceCollection AddConfigurationRoot(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IClock>(SystemClock.Instance);
            serviceCollection.AddScoped<IUserQueries, UserQueries>();
            serviceCollection.AddScoped<IRoleQueries, RoleQueries>();

            serviceCollection.AddScoped<IUserRepository, UserRepository>();
            serviceCollection.AddScoped<IRoleRepository, RoleRepository>();

            serviceCollection.AddScoped<ICurrentAuthenticatedUserProvider, CurrentAuthenticatedUserProvider>();
            serviceCollection.AddScoped<IAuthenticationService, AuthenticationService>();

            serviceCollection.AddSingleton<IFido2>(ConfigureFido);
            return serviceCollection;
        }

        private static IFido2 ConfigureFido(IServiceProvider arg)
        {
            var options = arg.GetService<IOptions<SecuritySettings>>();

            return new Fido2(new Fido2Configuration()
            {
                ServerDomain = options.Value.ServerDomain,
                ServerName = options.Value.SiteName,
                Origin = options.Value.Origin,
            });
        }
    }
}