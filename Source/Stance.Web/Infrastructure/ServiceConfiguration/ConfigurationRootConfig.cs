// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Stance.Core.Contracts;
using Stance.Domain.AggregatesModel.UserAggregate;
using Stance.Infrastructure.Repositories;
using Stance.Queries;
using Stance.Queries.Contracts;

namespace Stance.Web.Infrastructure.ServiceConfiguration
{
    public static class ConfigurationRootConfig
    {
        public static IServiceCollection AddConfigurationRoot(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IClock>(SystemClock.Instance);
            serviceCollection.AddScoped<IUserQueries, UserQueries>();
            serviceCollection.AddScoped<IUserRepository, UserRepository>();
            serviceCollection.AddScoped<ICurrentAuthenticatedUserProvider, CurrentAuthenticatedUserProvider>();
            return serviceCollection;
        }
    }
}