// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Destructurama;
using FluentValidation;
using Initium.Portal.Core.Database;
using Initium.Portal.Domain.AggregatesModel.NotificationAggregate;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using Initium.Portal.Domain.AggregatesModel.SystemAlertAggregate;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.CommandValidators.UserAggregate;
using Initium.Portal.Infrastructure.Behaviors;
using Initium.Portal.Infrastructure.EntityTypeConfigurationProviders;
using Initium.Portal.Infrastructure.Repositories;
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
                scan.FromAssemblyOf<AuthenticateUserCommand>()
                    .AddClasses(filter => filter.Where(t => t.BaseType == typeof(AbstractValidator<>)))
                    .AsMatchingInterface());

            return serviceCollection;
        }

        public static IServiceCollection AddCoreRepositories(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IUserRepository, UserRepository>();
            serviceCollection.AddScoped<IRoleRepository, RoleRepository>();
            serviceCollection.AddScoped<INotificationRepository, NotificationRepository>();
            serviceCollection.AddScoped<ISystemAlertRepository, SystemAlertRepository>();
            return serviceCollection;
        }

        public static IServiceCollection AddCoreEntityTypeConfigurationProviders(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, NotificationEntityTypeConfigurationProvider>();
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, RoleEntityTypeConfigurationProvider>();
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, SystemAlertEntityTypeConfigurationProvider>();
            serviceCollection.AddScoped<IEntityTypeConfigurationProvider, UserEntityTypeConfigurationProvider>();
            return serviceCollection;
        }
    }
}