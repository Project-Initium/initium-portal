// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Initium.Portal.Queries
{
    public sealed class QueryContext : DbContext
    {
        public QueryContext(DbContextOptions<QueryContext> options)
            : base(options)
        {
            this.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<UserNotification> UserNotifications { get; set; }

        public DbSet<SystemAlert> SystemAlerts { get; set; }

        public DbSet<Resource> Resources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resource>(ConfigureResource);
            modelBuilder.Entity<Role>(ConfigureRole);
            modelBuilder.Entity<RoleResource>(ConfigureRoleResource);
            modelBuilder.Entity<SystemAlert>(ConfigureSystemAlert);
            modelBuilder.Entity<User>(ConfigureUser);
            modelBuilder.Entity<UserRole>(ConfigureUserRole);
        }

        private static void ConfigureRoleResource(EntityTypeBuilder<RoleResource> roleResources)
        {
            roleResources.ToTable("vwRoleResource", "Portal");
            roleResources.HasKey(roleResource => new { roleResource.RoleId, roleResource.ResourceId });

            roleResources
                .HasOne(roleResource => roleResource.Resource)
                .WithMany(resource => resource.RoleResources)
                .HasForeignKey(roleResource => roleResource.ResourceId);

            roleResources
                .HasOne(roleResource => roleResource.Role)
                .WithMany(role => role.RoleResources)
                .HasForeignKey(roleResource => roleResource.RoleId);
        }

        private static void ConfigureResource(EntityTypeBuilder<Resource> resources)
        {
            resources.ToTable("vwResource", "Portal");
            resources.HasKey(resource => resource.Id);
        }

        private static void ConfigureSystemAlert(EntityTypeBuilder<SystemAlert> systemAlerts)
        {
            systemAlerts.ToTable("vwSystemAlert", "Portal");
            systemAlerts.HasKey(systemAlert => systemAlert.Id);
        }

        private static void ConfigureRole(EntityTypeBuilder<Role> roles)
        {
            roles.ToTable("vwRole", "Portal");
            roles.HasKey(role => role.Id);
        }

        private static void ConfigureUser(EntityTypeBuilder<User> users)
        {
            users.ToTable("vwUser", "Portal");
            users.HasKey(x => x.Id);

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

            users.OwnsMany(user => user.UserNotifications, userNotifications =>
            {
                userNotifications.ToTable("vwUserNotification", "Portal");
                userNotifications.HasKey(userNotification =>
                    new { userNotification.NotificationId, userNotification.UserId });
            });
        }

        private static void ConfigureUserRole(EntityTypeBuilder<UserRole> userRoles)
        {
            userRoles.ToTable("vwUserRole", "Portal");
            userRoles.HasKey(userRole => new { userRole.RoleId, userRole.UserId });

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