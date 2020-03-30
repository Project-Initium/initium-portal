// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Destructurama;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Stance.Domain.Commands.UserAggregate;

namespace Stance.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainAuditLogging(this IServiceCollection serviceCollection)
        {
            Log.Logger = new LoggerConfiguration()
                .Destructure.ByIgnoringProperties<AuthenticateUserCommand>(u => u.Password)
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
            serviceCollection.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuditBehavior<,>));
            return serviceCollection.AddSingleton(Log.Logger);
        }
    }
}