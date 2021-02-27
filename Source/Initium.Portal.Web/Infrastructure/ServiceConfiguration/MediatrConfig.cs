// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Initium.Portal.Domain.CommandHandlers.UserAggregate;
using Initium.Portal.Infrastructure.Extensions;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.DependencyInjection;

namespace Initium.Portal.Web.Infrastructure.ServiceConfiguration
{
    public static class MediatrConfig
    {
        public static IServiceCollection AddCoreCustomizedMediatR(
            this IServiceCollection serviceCollection,
            IEnumerable<Assembly> assembliesToAdd)
        {
            var assembly = typeof(AuthenticateUserCommandHandler).GetTypeInfo().Assembly;
            var completeList = assembliesToAdd.Append(assembly).ToArray();
            serviceCollection.AddMediatR(completeList);
            serviceCollection.AddFluentValidation(completeList, ServiceLifetime.Transient);
            serviceCollection.AddDomainAuditLogging();
            return serviceCollection;
        }
    }
}