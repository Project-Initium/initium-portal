using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stance.Infrastructure;

namespace Stance.Web.Infrastructure.ServiceConfiguration
{
    public static class DataStoreConfig
    {
        public static IServiceCollection AddDataStores(
            this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddDistributedMemoryCache();
            services.AddEntityFrameworkSqlServer()
                .AddDbContext<DataContext>(options =>
                {
                    options.UseSqlServer(configuration["query:connectionString"]);
                });

            return services;
        }
    }
}