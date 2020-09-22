// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Runtime.CompilerServices;
using Finbuckle.MultiTenant;
using Initium.Portal.Core.Settings;
using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Options;

[assembly: InternalsVisibleTo("Initium.Portal.Tests")]

namespace Initium.Portal.Queries
{
    public sealed class QueryContext : DbContext
    {
        private readonly ITenantInfo _tenantInfo;
        private readonly MultiTenantSettings _multiTenantSettings;

        public QueryContext(ITenantInfo tenantInfo, IOptions<MultiTenantSettings> multiTenantSettings)
        {
            if (multiTenantSettings == null)
            {
                throw new ArgumentNullException(nameof(multiTenantSettings));
            }

            this._tenantInfo = tenantInfo ?? throw new ArgumentNullException(nameof(tenantInfo));
            this._multiTenantSettings = multiTenantSettings.Value;
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        internal QueryContext(DbContextOptions<QueryContext> options, ITenantInfo tenantInfo, IOptions<MultiTenantSettings> multiTenantSettings)
            : base(options)
        {
            this._tenantInfo = tenantInfo ?? throw new ArgumentNullException(nameof(tenantInfo));
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            this._multiTenantSettings = multiTenantSettings.Value;
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserNotification> UserNotifications { get; set; }

        public DbSet<SystemAlert> SystemAlerts { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<TenantDto> Tenants { get; set; }

        public DbSet<RoleResource> RoleResources { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }

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
            modelBuilder.Entity<Resource>(this.ConfigureResource);
            modelBuilder.Entity<Role>(this.ConfigureRole);
            modelBuilder.Entity<RoleResource>(this.ConfigureRoleResource);
            modelBuilder.Entity<SystemAlert>(this.ConfigureSystemAlert);
            modelBuilder.Entity<User>(this.ConfigureUser);
            modelBuilder.Entity<UserRole>(this.ConfigureUserRole);
            modelBuilder.Entity<UserNotification>(this.ConfigureUserNotification);
            modelBuilder.Entity<TenantDto>(this.ConfigureTenant);
        }

        private void ConfigureTenant(EntityTypeBuilder<TenantDto> tenants)
        {
            tenants.ToTable("vwTenant", "Portal");
            tenants.HasKey(tenant => tenant.Id);
            tenants.HasQueryFilter(tenant => tenant.Id != this._multiTenantSettings.DefaultTenantId);
        }

        private void ConfigureUserNotification(EntityTypeBuilder<UserNotification> userNotifications)
        {
            userNotifications.ToTable("vwUserNotification", "Portal");
            userNotifications.HasKey(userNotification =>
                new { userNotification.NotificationId, userNotification.UserId });
            userNotifications.HasOne(x => x.User).WithMany(x => x.UserNotifications).HasForeignKey(x => x.UserId);
            userNotifications.Property<Guid>("TenantId");
            userNotifications.HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(this._tenantInfo.Id));
        }

        private void ConfigureRoleResource(EntityTypeBuilder<RoleResource> roleResources)
        {
            roleResources.ToTable("vwRoleResource", "Portal");
            roleResources.HasKey(roleResource => new { roleResource.RoleId, roleResource.ResourceId });
            roleResources.Property<Guid>("TenantId");
            roleResources.HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(this._tenantInfo.Id));

            roleResources
                .HasOne(roleResource => roleResource.Resource)
                .WithMany(resource => resource.RoleResources)
                .HasForeignKey(roleResource => roleResource.ResourceId);

            roleResources
                .HasOne(roleResource => roleResource.Role)
                .WithMany(role => role.RoleResources)
                .HasForeignKey(roleResource => roleResource.RoleId);
        }

        private void ConfigureResource(EntityTypeBuilder<Resource> resources)
        {
            resources.ToTable("vwResource", "Portal");
            resources.HasKey(resource => resource.Id);
        }

        private void ConfigureSystemAlert(EntityTypeBuilder<SystemAlert> systemAlerts)
        {
            systemAlerts.ToTable("vwSystemAlert", "Portal");
            systemAlerts.HasKey(systemAlert => systemAlert.Id);

            systemAlerts.Property<Guid>("TenantId");
            systemAlerts.HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(this._tenantInfo.Id));
        }

        private void ConfigureRole(EntityTypeBuilder<Role> roles)
        {
            roles.ToTable("vwRole", "Portal");
            roles.HasKey(role => role.Id);
            roles.Property<Guid>("TenantId");
            roles.HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(this._tenantInfo.Id));
        }

        private void ConfigureUser(EntityTypeBuilder<User> users)
        {
            users.ToTable("vwUser", "portal");
            users.HasKey(x => x.Id);
            users.Property<Guid>("TenantId");
            users.HasQueryFilter(e => EF.Property<Guid>(e, "TenantId") == Guid.Parse(this._tenantInfo.Id));

            users.OwnsMany(user => user.AuthenticatorApps, authenticatorApps =>
             {
                 authenticatorApps.ToTable("vwAuthenticatorApp", "Portal");
                 authenticatorApps.HasKey(authenticatorApp => authenticatorApp.Id);
             });

            users.OwnsMany(user => user.AuthenticatorDevices, authenticatorDevices =>
             {
                 authenticatorDevices.ToTable("vwAuthenticatorDevice", "Portal");
                 authenticatorDevices.HasKey(authenticatorDevice => authenticatorDevice.Id);
             });

            users.HasMany<UserNotification>().WithOne(x => x.User).HasForeignKey(x => x.UserId);
        }

        private void ConfigureUserRole(EntityTypeBuilder<UserRole> userRoles)
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