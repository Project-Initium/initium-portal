// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Reflection;
using Destructurama;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.CommandValidators.UserAggregate;
using Stance.Infrastructure.Behaviors;

namespace Stance.Infrastructure.Extensions
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