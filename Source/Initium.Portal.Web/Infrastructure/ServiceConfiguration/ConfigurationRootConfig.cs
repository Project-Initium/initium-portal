// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Fido2NetLib;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Settings;
using Initium.Portal.Core.Workers;
using Initium.Portal.Domain.AggregatesModel.NotificationAggregate;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Infrastructure.Repositories;
using Initium.Portal.Queries.Contracts.Static;
using Initium.Portal.Queries.Static;
using Initium.Portal.Web.Infrastructure.Contracts;
using Initium.Portal.Web.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NodaTime;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
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
            serviceCollection.AddScoped<INotificationRepository, NotificationRepository>();
            serviceCollection.AddScoped<IConnectionProvider, ConnectionProvider>();

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