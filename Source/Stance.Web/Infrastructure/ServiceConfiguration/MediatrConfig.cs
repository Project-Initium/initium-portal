// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Reflection;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;
using Stance.Domain.CommandHandlers.UserAggregate;
using Stance.Infrastructure;
using Stance.Infrastructure.Extensions;

namespace Stance.Web.Infrastructure.ServiceConfiguration
{
    public static class MediatrConfig
    {
        public static IServiceCollection AddCustomizedMediatR(this IServiceCollection serviceCollection)
        {
            var assembly = typeof(CreateInitialUserCommandHandler).GetTypeInfo().Assembly;
            serviceCollection.AddMediatR(assembly);
            serviceCollection.AddFluentValidation(new[] { assembly });
            serviceCollection.AddDomainAuditLogging();
            return serviceCollection;
        }
    }
}