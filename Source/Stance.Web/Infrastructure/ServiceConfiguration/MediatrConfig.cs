using System.Reflection;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Stance.Domain.CommandHandlers.UserAggregate;

namespace Stance.Web.Infrastructure.ServiceConfiguration
{
    public static class MediatrConfig
    {
        public static IServiceCollection AddCustomizedMediatR(this IServiceCollection serviceCollection)
        {
            var assembly = typeof(CreateInitialUserCommandHandler).GetTypeInfo().Assembly;
            serviceCollection.AddMediatR(assembly);
            serviceCollection.AddFluentValidation(new[] { assembly });
            return serviceCollection;
        }
    }
}