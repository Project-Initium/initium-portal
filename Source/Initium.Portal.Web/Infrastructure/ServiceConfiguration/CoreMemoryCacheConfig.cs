using Initium.Portal.Core.Extensions;
using Initium.Portal.Core.Serialization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class CoreMemoryCacheConfig
    {
        public static IServiceCollection AddMemoryCacheSettings(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddScoped<IDataSerializer, ProtoBufDataSerializer>();
            services.AddScoped<ICustomDistributedCache, CustomDistributedCache>();
            return services;
        }
    }
}