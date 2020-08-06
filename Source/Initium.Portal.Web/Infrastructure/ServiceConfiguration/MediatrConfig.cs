// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Reflection;
using Initium.Portal.Domain.CommandHandlers.UserAggregate;
using Initium.Portal.Infrastructure.Extensions;
using Initium.Portal.Web.Infrastructure.DomainEventHandlers;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class MediatrConfig
    {
        public static IServiceCollection AddCustomizedMediatR(this IServiceCollection serviceCollection)
        {
            var assembly = typeof(CreateInitialUserCommandHandler).GetTypeInfo().Assembly;
            serviceCollection.AddMediatR(typeof(CreateInitialUserCommandHandler), typeof(LogEmailMfaToken));
            serviceCollection.AddFluentValidation(new[] { assembly });
            serviceCollection.AddDomainAuditLogging();
            return serviceCollection;
        }
    }
}