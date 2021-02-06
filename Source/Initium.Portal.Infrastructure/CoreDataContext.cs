// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.AggregatesModel.NotificationAggregate;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using Initium.Portal.Domain.AggregatesModel.SystemAlertAggregate;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Infrastructure.Extensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.DependencyInjection;

[assembly: InternalsVisibleTo("Initium.Portal.Tests")]

namespace Initium.Portal.Infrastructure
{
    public abstract class CoreDataContext : DbContext, ICoreDataContext
    {
        private readonly FeatureBasedTenantInfo _tenantInfo;
        private readonly IMediator _mediator;
        private readonly IServiceProvider _serviceProvider;

        protected CoreDataContext(FeatureBasedTenantInfo tenantInfo, IMediator mediator, IServiceProvider serviceProvider)
        {
            this._tenantInfo = tenantInfo;
            this._mediator = mediator;
            this._serviceProvider = serviceProvider;
        }

        protected CoreDataContext(DbContextOptions<CoreDataContext> options, IMediator mediator, FeatureBasedTenantInfo tenantInfo, IServiceProvider serviceProvider)
            : base(options)
        {
            this._mediator = mediator;
            this._tenantInfo = tenantInfo;
            this._serviceProvider = serviceProvider;
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<SystemAlert> SystemAlerts { get; set; }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await this._mediator.DispatchDomainEventsAsync(this);
            await this.SaveChangesAsync(cancellationToken);
            await this._mediator.DispatchIntegrationEventsAsync(this);
            return true;
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in this.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added && entry.Metadata.FindAnnotation("MULTI_TENANT") != null)
                {
                    entry.Property("TenantId").CurrentValue = Guid.Parse(this._tenantInfo.Id);
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(this._tenantInfo.ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            var providers = this._serviceProvider.GetServices<IEntityTypeConfigurationProvider>();
            foreach (var entityTypeConfigurationProvider in providers)
            {
                entityTypeConfigurationProvider.ApplyConfigurations(modelBuilder);
            }

            modelBuilder.Entity<User>(this.ConfigureUser);
            modelBuilder.Entity<Role>(this.ConfigureRole);
            modelBuilder.Entity<Notification>(this.ConfigureNotification);
            modelBuilder.Entity<SystemAlert>(this.ConfigureSystemAlert);
        }

        private void ConfigureSystemAlert(EntityTypeBuilder<SystemAlert> systemAlerts)
        {
            systemAlerts.ToTable("SystemAlert", "Messaging");
            systemAlerts.HasKey(systemAlert => systemAlert.Id);
            systemAlerts.Ignore(systemAlert => systemAlert.DomainEvents);
            systemAlerts.Ignore(systemAlert => systemAlert.IntegrationEvents);
            systemAlerts.Property(systemAlert => systemAlert.Id).ValueGeneratedNever();
            systemAlerts.Metadata.AddAnnotation("MULTI_TENANT", null);
            systemAlerts.Property<Guid>("TenantId");
            systemAlerts.HasQueryFilter(systemAlert => EF.Property<Guid>(systemAlert, "TenantId") == Guid.Parse(this._tenantInfo.Id));
        }

        private void ConfigureNotification(EntityTypeBuilder<Notification> notifications)
        {
            notifications.ToTable("Notification", "Messaging");
            notifications.HasKey(notification => notification.Id);
            notifications.Ignore(notification => notification.DomainEvents);
            notifications.Ignore(notification => notification.IntegrationEvents);
            notifications.Property(notification => notification.Id).ValueGeneratedNever();
            notifications.Metadata.AddAnnotation("MULTI_TENANT", null);
            notifications.Property<Guid>("TenantId");
            notifications.HasQueryFilter(notification => EF.Property<Guid>(notification, "TenantId") == Guid.Parse(this._tenantInfo.Id));

            var navigation = notifications.Metadata.FindNavigation(nameof(Notification.UserNotifications));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            notifications.OwnsMany(role => role.UserNotifications, userNotifications =>
            {
                userNotifications.ToTable("vwUserNotification", "Messaging");
                userNotifications.HasKey(userNotification => userNotification.Id);
                userNotifications.Property(userNotification => userNotification.Id).ValueGeneratedNever();
                userNotifications.Ignore(userNotification => userNotification.DomainEvents);
                userNotifications.Ignore(userNotification => userNotification.IntegrationEvents);
                userNotifications.OwnedEntityType.AddAnnotation("MULTI_TENANT", null);
                userNotifications.Property<Guid>("TenantId");
            });
        }

        private void ConfigureRole(EntityTypeBuilder<Role> roles)
        {
            roles.ToTable("Role", "AccessProtection");
            roles.HasKey(role => role.Id);
            roles.Ignore(role => role.DomainEvents);
            roles.Ignore(role => role.IntegrationEvents);
            roles.Property(role => role.Id).ValueGeneratedNever();
            roles.Metadata.AddAnnotation("MULTI_TENANT", null);
            roles.Property<Guid>("TenantId");
            roles.HasQueryFilter(role => EF.Property<Guid>(role, "TenantId") == Guid.Parse(this._tenantInfo.Id));

            var navigation = roles.Metadata.FindNavigation(nameof(Role.RoleResources));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            roles.OwnsMany(role => role.RoleResources, roleResources =>
            {
                roleResources.ToTable("RoleResource", "AccessProtection");
                roleResources.HasKey(entity => entity.Id);
                roleResources.Property(roleResource => roleResource.Id).ValueGeneratedNever();
                roleResources.Property(roleResource => roleResource.Id).HasColumnName("ResourceId");
                roleResources.Ignore(roleResource => roleResource.DomainEvents);
                roleResources.Ignore(roleResource => roleResource.IntegrationEvents);
                roleResources.OwnedEntityType.AddAnnotation("MULTI_TENANT", null);
                roleResources.Property<Guid>("TenantId");
            });
        }

        private void ConfigureUser(EntityTypeBuilder<User> users)
        {
            users.ToTable("User", "Identity");
            users.HasKey(user => user.Id);
            users.Ignore(user => user.DomainEvents);
            users.Ignore(user => user.IntegrationEvents);
            users.Property(user => user.Id).ValueGeneratedNever();
            users.Metadata.AddAnnotation("MULTI_TENANT", null);
            users.Property<Guid>("TenantId");
            users.HasQueryFilter(user => EF.Property<Guid>(user, "TenantId") == Guid.Parse(this._tenantInfo.Id));

            var navigation = users.Metadata.FindNavigation(nameof(User.AuthenticationHistories));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany(user => user.AuthenticationHistories, authenticationHistories =>
            {
                authenticationHistories.ToTable("AuthenticationHistory", "Identity");
                authenticationHistories.HasKey(authenticationHistory => authenticationHistory.Id);
                authenticationHistories.Property(authenticationHistory => authenticationHistory.Id)
                    .ValueGeneratedNever();
                authenticationHistories.Ignore(authenticationHistory => authenticationHistory.DomainEvents);
                authenticationHistories.Ignore(authenticationHistory => authenticationHistory.IntegrationEvents);
                authenticationHistories.OwnedEntityType.AddAnnotation("MULTI_TENANT", null);
                authenticationHistories.Property<Guid>("TenantId");
            });

            navigation = users.Metadata.FindNavigation(nameof(User.SecurityTokenMappings));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany(user => user.SecurityTokenMappings, securityTokenMappings =>
            {
                securityTokenMappings.ToTable("SecurityTokenMapping", "Identity");
                securityTokenMappings.HasKey(securityTokenMapping => securityTokenMapping.Id);
                securityTokenMappings.Property(securityTokenMapping => securityTokenMapping.Id)
                    .ValueGeneratedNever();
                securityTokenMappings.Ignore(securityTokenMapping => securityTokenMapping.DomainEvents);
                securityTokenMappings.Ignore(securityTokenMapping => securityTokenMapping.IntegrationEvents);
                securityTokenMappings.OwnedEntityType.AddAnnotation("MULTI_TENANT", null);
                securityTokenMappings.Property<Guid>("TenantId");
            });

            users.OwnsOne(user => user.Profile, profile =>
            {
                profile.ToTable("Profile", "Identity");
                profile.WithOwner().HasForeignKey(x => x.Id);
                profile.HasKey(item => item.Id);
                profile.Property(item => item.Id).HasColumnName("UserId");
                profile.Ignore(item => item.DomainEvents);
                profile.Ignore(item => item.IntegrationEvents);
                profile.OwnedEntityType.AddAnnotation("MULTI_TENANT", null);
                profile.Property<Guid>("TenantId");
            });

            navigation = users.Metadata.FindNavigation(nameof(User.UserRoles));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany(user => user.UserRoles, userRoles =>
            {
                userRoles.ToTable("UserRole", "Identity");
                userRoles.Property(userRole => userRole.Id).ValueGeneratedNever();
                userRoles.Property(userRole => userRole.Id).HasColumnName("RoleId");
                userRoles.HasKey("Id", "UserId");
                userRoles.Ignore(userRole => userRole.DomainEvents);
                userRoles.Ignore(userRole => userRole.IntegrationEvents);
                userRoles.OwnedEntityType.AddAnnotation("MULTI_TENANT", null);
                userRoles.Property<Guid>("TenantId");
            });

            navigation = users.Metadata.FindNavigation(nameof(User.AuthenticatorApps));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany(user => user.AuthenticatorApps, authenticatorApps =>
            {
                authenticatorApps.ToTable("AuthenticatorApp", "Identity");
                authenticatorApps.HasKey(authenticatorApp => authenticatorApp.Id);
                authenticatorApps.Property(authenticatorApp => authenticatorApp.Id)
                    .ValueGeneratedNever();
                authenticatorApps.Ignore(authenticatorApp => authenticatorApp.DomainEvents);
                authenticatorApps.Ignore(authenticatorApp => authenticatorApp.IntegrationEvents);
                authenticatorApps.OwnedEntityType.AddAnnotation("MULTI_TENANT", null);
                authenticatorApps.Property<Guid>("TenantId");
            });

            navigation = users.Metadata.FindNavigation(nameof(User.AuthenticatorDevices));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany(user => user.AuthenticatorDevices, authenticatorDevices =>
            {
                authenticatorDevices.ToTable("AuthenticatorDevice", "Identity");
                authenticatorDevices.HasKey(authenticatorDevice => authenticatorDevice.Id);
                authenticatorDevices.Property(authenticatorDevice => authenticatorDevice.Id)
                    .ValueGeneratedNever();
                authenticatorDevices.Ignore(authenticatorDevice => authenticatorDevice.DomainEvents);
                authenticatorDevices.Ignore(authenticatorDevice => authenticatorDevice.IntegrationEvents);
                authenticatorDevices.OwnedEntityType.AddAnnotation("MULTI_TENANT", null);
                authenticatorDevices.Property<Guid>("TenantId");
            });

            navigation = users.Metadata.FindNavigation(nameof(User.PasswordHistories));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany(user => user.PasswordHistories, passwordHistories =>
            {
                passwordHistories.ToTable("PasswordHistory", "Identity");
                passwordHistories.HasKey(passwordHistory => passwordHistory.Id);
                passwordHistories.Property(passwordHistory => passwordHistory.Id)
                    .ValueGeneratedNever();
                passwordHistories.Ignore(passwordHistory => passwordHistory.DomainEvents);
                passwordHistories.Ignore(passwordHistory => passwordHistory.IntegrationEvents);
                passwordHistories.OwnedEntityType.AddAnnotation("MULTI_TENANT", null);
                passwordHistories.Property<Guid>("TenantId");
            });

            navigation = users.Metadata.FindNavigation(nameof(User.UserNotifications));
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany(user => user.UserNotifications, userNotifications =>
            {
                userNotifications.ToTable("UserNotification", "Messaging");
                userNotifications.HasKey(userNotification => userNotification.Id);
                userNotifications.Property(userNotification => userNotification.Id)
                    .ValueGeneratedNever();
                userNotifications.Property(userNotification => userNotification.Id).HasColumnName("NotificationId");
                userNotifications.Ignore(userNotification => userNotification.DomainEvents);
                userNotifications.Ignore(userNotification => userNotification.IntegrationEvents);
                userNotifications.OwnedEntityType.AddAnnotation("MULTI_TENANT", null);
                userNotifications.Property<Guid>("TenantId");
            });
        }
    }
}