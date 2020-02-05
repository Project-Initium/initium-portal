using Microsoft.Extensions.DependencyInjection;
using NodaTime;
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
            return serviceCollection;
        }
    }
}