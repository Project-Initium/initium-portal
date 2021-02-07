// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Core.Settings;
using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;

[assembly: InternalsVisibleTo("Initium.Portal.Tests")]

namespace Initium.Portal.Queries
{
    public abstract class CoreQueryContext : DbContext, ICoreQueryContext
    {
        private readonly FeatureBasedTenantInfo _tenantInfo;
        private readonly MultiTenantSettings _multiTenantSettings;

        protected CoreQueryContext(FeatureBasedTenantInfo tenantInfo, IOptions<MultiTenantSettings> multiTenantSettings)
        {
            if (multiTenantSettings == null)
            {
                throw new ArgumentNullException(nameof(multiTenantSettings));
            }

            this._tenantInfo = tenantInfo ?? throw new ArgumentNullException(nameof(tenantInfo));
            this._multiTenantSettings = multiTenantSettings.Value;
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this._multiTenantSettings = multiTenantSettings.Value;
        }

        protected CoreQueryContext(DbContextOptions<CoreQueryContext> options, FeatureBasedTenantInfo tenantInfo, IOptions<MultiTenantSettings> multiTenantSettings)
            : base(options)
        {
            this._tenantInfo = tenantInfo ?? throw new ArgumentNullException(nameof(tenantInfo));
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this._multiTenantSettings = multiTenantSettings.Value;
        }

        public DbSet<UserReadEntity> Users { get; set; }

        public DbSet<RoleReadEntity> Roles { get; set; }

        public DbSet<UserNotificationReadEntity> UserNotifications { get; set; }

        public DbSet<SystemAlertReadEntity> SystemAlerts { get; set; }

        public DbSet<ResourceReadEntity> Resources { get; set; }

        public DbSet<RoleResourceReadEntity> RoleResources { get; set; }

        public DbSet<UserRoleReadEntity> UserRoles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
            {
                return;
            }

            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseSqlServer(this._tenantInfo.ConnectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResourceReadEntity>(this.ConfigureResource);
            modelBuilder.Entity<RoleReadEntity>(this.ConfigureRole);
            modelBuilder.Entity<RoleResourceReadEntity>(this.ConfigureRoleResource);
            modelBuilder.Entity<SystemAlertReadEntity>(this.ConfigureSystemAlert);
            modelBuilder.Entity<UserReadEntity>(this.ConfigureUser);
            modelBuilder.Entity<UserRoleReadEntity>(this.ConfigureUserRole);
            modelBuilder.Entity<>(this.ConfigureUserNotification);
        }

        private void ConfigureUserNotification(EntityTypeBuilder<UserNotificationReadEntity> userNotifications)
        {
            
        }

        private void ConfigureRoleResource(EntityTypeBuilder<RoleResourceReadEntity> roleResources)
        {
            
        }

        private void ConfigureResource(EntityTypeBuilder<ResourceReadEntity> resources)
        {
            resources.ToTable("vwResource", "Portal");
            resources.HasKey(resource => resource.Id);
        }

        private void ConfigureSystemAlert(EntityTypeBuilder<SystemAlertReadEntity> systemAlerts)
        {
            systemAlerts.ToTable("vwSystemAlert", "Portal");
            systemAlerts.HasKey(systemAlert => systemAlert.Id);

            systemAlerts.Property<Guid>("TenantId");
            systemAlerts.HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(this._tenantInfo.Id));
        }

        private void ConfigureRole(EntityTypeBuilder<RoleReadEntity> roles)
        {
            roles.ToTable("vwRole", "Portal");
            roles.HasKey(role => role.Id);
            roles.Property<Guid>("TenantId");
            roles.HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(this._tenantInfo.Id));
        }

        private void ConfigureUser(EntityTypeBuilder<UserReadEntity> users)
        {
            users.ToTable("vwUser", "portal");
            users.HasKey(x => x.Id);
            users.Property<Guid>("TenantId");
            users.HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(this._tenantInfo.Id));

            users.OwnsMany(user => user.AuthenticatorApps, authenticatorApps =>
             {
                 authenticatorApps.ToTable("vwAuthenticatorApp", "Portal");
                 authenticatorApps.HasKey(authenticatorApp => authenticatorApp.Id);
             }).UsePropertyAccessMode(PropertyAccessMode.Field);

            users.OwnsMany(user => user.AuthenticatorDevices, authenticatorDevices =>
             {
                 authenticatorDevices.ToTable("vwAuthenticatorDevice", "Portal");
                 authenticatorDevices.HasKey(authenticatorDevice => authenticatorDevice.Id);
             }).UsePropertyAccessMode(PropertyAccessMode.Field);

            users
                .HasMany<UserNotificationReadEntity>()
                .WithOne(x => x.User)
                .HasForeignKey(x => x.UserId);
            users.Metadata.FindNavigation(nameof(UserReadEntity.UserNotifications)).SetPropertyAccessMode(PropertyAccessMode.Field);
        }

        private void ConfigureUserRole(EntityTypeBuilder<UserRoleReadEntity> userRoles)
        {
            userRoles.ToTable("vwUserRole", "Portal");
            userRoles.HasKey(userRole => new { userRole.RoleId, userRole.UserId });
            userRoles.Property<Guid>("TenantId");
            userRoles.HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(this._tenantInfo.Id));

            userRoles
                .HasOne(userRole => userRole.Role)
                .WithMany(role => role.UserRoles)
                .HasForeignKey(userRole => userRole.RoleId);

            userRoles
                .HasOne(userRole => userRole.User)
                .WithMany(user => user.UserRoles)
                .HasForeignKey(userRole => userRole.UserId);
        }
    }
}