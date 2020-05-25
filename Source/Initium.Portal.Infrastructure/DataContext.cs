// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.AggregatesModel.NotificationAggregate;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using Initium.Portal.Domain.AggregatesModel.SystemAlertAggregate;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

[assembly: InternalsVisibleTo("Initium.Portal.Tests")]

namespace Initium.Portal.Infrastructure
{
    public sealed class DataContext : DbContext, IUnitOfWork
    {
        private readonly IMediator _mediator;

        public DataContext(DbContextOptions<DataContext> options, IMediator mediator)
            : base(options)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<SystemAlert> SystemAlerts { get; set; }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await this.SaveChangesAsync(cancellationToken);
            await this._mediator.DispatchDomainEventsAsync(this);
            return true;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(this.ConfigureUser);
            modelBuilder.Entity<Role>(this.ConfigureRole);
            modelBuilder.Entity<Notification>(this.ConfigureNotification);
            modelBuilder.Entity<SystemAlert>(this.ConfigureSystemAlert);
        }

        private void ConfigureSystemAlert(EntityTypeBuilder<SystemAlert> systemAlerts)
        {
            systemAlerts.ToTable("systemAlert", "messaging");
            systemAlerts.HasKey(systemAlert => systemAlert.Id);
            systemAlerts.Ignore(systemAlert => systemAlert.DomainEvents);
            systemAlerts.Property(systemAlert => systemAlert.Id).ValueGeneratedNever();
        }

        private void ConfigureNotification(EntityTypeBuilder<Notification> notifications)
        {
            notifications.ToTable("notification", "messaging");
            notifications.HasKey(notification => notification.Id);
            notifications.Ignore(notification => notification.DomainEvents);
            notifications.Property(notification => notification.Id).ValueGeneratedNever();

            var navigation = notifications.Metadata.FindNavigation(nameof(Notification.UserNotifications));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            notifications.OwnsMany(role => role.UserNotifications, userNotifications =>
            {
                userNotifications.ToTable("vwUserNotification", "messaging");
                userNotifications.HasKey(userNotification => userNotification.Id);
                userNotifications.Property(userNotification => userNotification.Id).ValueGeneratedNever();
                userNotifications.Ignore(userNotification => userNotification.DomainEvents);
            });
        }

        private void ConfigureRole(EntityTypeBuilder<Role> roles)
        {
            roles.ToTable("role", "accessProtection");
            roles.HasKey(entity => entity.Id);
            roles.Ignore(b => b.DomainEvents);
            roles.Property(e => e.Id).ValueGeneratedNever();

            var navigation = roles.Metadata.FindNavigation(nameof(Role.RoleResources));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            roles.OwnsMany(role => role.RoleResources, roleResources =>
            {
                roleResources.ToTable("roleResource", "accessProtection");
                roleResources.HasKey(entity => entity.Id);
                roleResources.Property(e => e.Id).ValueGeneratedNever();
                roleResources.Property(x => x.Id).HasColumnName("resourceId");
                roleResources.Ignore(b => b.DomainEvents);
            });
        }

        private void ConfigureUser(EntityTypeBuilder<User> users)
        {
            users.ToTable("user", "identity");
            users.HasKey(entity => entity.Id);
            users.Ignore(b => b.DomainEvents);
            users.Property(e => e.Id).ValueGeneratedNever();

            var navigation = users.Metadata.FindNavigation(nameof(User.AuthenticationHistories));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany<AuthenticationHistory>(user => user.AuthenticationHistories, authenticationHistories =>
            {
                authenticationHistories.ToTable("authenticationHistory", "identity");
                authenticationHistories.HasKey(authenticationHistory => authenticationHistory.Id);
                authenticationHistories.Property(authenticationHistory => authenticationHistory.Id)
                    .ValueGeneratedNever();
                authenticationHistories.Ignore(authenticationHistory => authenticationHistory.DomainEvents);
            });

            navigation = users.Metadata.FindNavigation(nameof(User.SecurityTokenMappings));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany<SecurityTokenMapping>(user => user.SecurityTokenMappings, securityTokenMappings =>
            {
                securityTokenMappings.ToTable("securityTokenMapping", "identity");
                securityTokenMappings.HasKey(securityTokenMapping => securityTokenMapping.Id);
                securityTokenMappings.Property(securityTokenMapping => securityTokenMapping.Id)
                    .ValueGeneratedNever();
                securityTokenMappings.Ignore(securityTokenMapping => securityTokenMapping.DomainEvents);
            });

            users.OwnsOne(user => user.Profile, profile =>
            {
                profile.ToTable("profile", "identity");
                profile.WithOwner().HasForeignKey(x => x.Id);
                profile.HasKey(item => item.Id);
                profile.Property(item => item.Id).HasColumnName("userId");
                profile.Ignore(item => item.DomainEvents);
            });

            navigation = users.Metadata.FindNavigation(nameof(User.UserRoles));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany(user => user.UserRoles, userRoles =>
            {
                userRoles.ToTable("userRole", "identity");
                userRoles.HasKey(userRole => userRole.Id);
                userRoles.Property(userRole => userRole.Id).ValueGeneratedNever();
                userRoles.Property(userRole => userRole.Id).HasColumnName("roleId");
                userRoles.Ignore(userRole => userRole.DomainEvents);
            });

            navigation = users.Metadata.FindNavigation(nameof(User.AuthenticatorApps));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany(user => user.AuthenticatorApps, authenticatorApps =>
            {
                authenticatorApps.ToTable("authenticatorApp", "identity");
                authenticatorApps.HasKey(authenticatorApp => authenticatorApp.Id);
                authenticatorApps.Property(authenticatorApp => authenticatorApp.Id)
                    .ValueGeneratedNever();
                authenticatorApps.Ignore(authenticatorApp => authenticatorApp.DomainEvents);
            });

            navigation = users.Metadata.FindNavigation(nameof(User.AuthenticatorDevices));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany(user => user.AuthenticatorDevices, authenticatorDevices =>
            {
                authenticatorDevices.ToTable("authenticatorDevice", "identity");
                authenticatorDevices.HasKey(authenticatorDevice => authenticatorDevice.Id);
                authenticatorDevices.Property(authenticatorDevice => authenticatorDevice.Id)
                    .ValueGeneratedNever();
                authenticatorDevices.Ignore(authenticatorDevice => authenticatorDevice.DomainEvents);
            });

            navigation = users.Metadata.FindNavigation(nameof(User.PasswordHistories));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany(user => user.PasswordHistories, passwordHistories =>
            {
                passwordHistories.ToTable("passwordHistory", "identity");
                passwordHistories.HasKey(passwordHistory => passwordHistory.Id);
                passwordHistories.Property(passwordHistory => passwordHistory.Id)
                    .ValueGeneratedNever();
                passwordHistories.Ignore(passwordHistory => passwordHistory.DomainEvents);
            });

            navigation = users.Metadata.FindNavigation(nameof(User.UserNotifications));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany(user => user.UserNotifications, userNotifications =>
            {
                userNotifications.ToTable("userNotification", "messaging");
                userNotifications.HasKey(userNotification => userNotification.Id);
                userNotifications.Property(userNotification => userNotification.Id)
                    .ValueGeneratedNever();
                userNotifications.Property(userNotification => userNotification.Id).HasColumnName("notificationId");
                userNotifications.Ignore(userNotification => userNotification.DomainEvents);
            });
        }
    }
}