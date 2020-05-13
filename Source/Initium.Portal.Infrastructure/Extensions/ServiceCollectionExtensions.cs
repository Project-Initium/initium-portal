// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Destructurama;
using FluentValidation;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.CommandValidators.UserAggregate;
using Initium.Portal.Infrastructure.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Initium.Portal.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDomainAuditLogging(this IServiceCollection serviceCollection)
        {
            Log.Logger = new LoggerConfiguration()
                .Destructure.ByIgnoringProperties<AuthenticateUserCommand>(u => u.Password)
                .Destructure.ByIgnoringProperties<PasswordResetCommand>(u => u.NewPassword)
                .Destructure.ByIgnoringProperties<ChangePasswordCommand>(u => u.NewPassword)
                .Destructure.ByIgnoringProperties<ChangePasswordCommand>(u => u.CurrentPassword)
                .Destructure.ByIgnoringProperties<CreateInitialUserCommand>(u => u.Password)
                .Destructure.ByIgnoringProperties<VerifyAccountAndSetPasswordCommand>(u => u.NewPassword)
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();
            serviceCollection.AddScoped(typeof(IPipelineBehavior<,>), typeof(AuditBehavior<,>));
            return serviceCollection.AddSingleton(Log.Logger);
        }

        public static IServiceCollection AddDomainValidation(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped(typeof(ValidatorBehavior<,>), typeof(IPipelineBehavior<,>));
            serviceCollection.Scan(scan =>
                scan.FromAssemblyOf<CreateInitialUserCommandValidator>()
                    .AddClasses(filter => filter.Where(t => t.BaseType == typeof(AbstractValidator<>)))
                    .AsMatchingInterface());

            return serviceCollection;
        }
    }
}