using System;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Infrastructure.EntityTypeConfigurationProviders
{
    public class UserEntityTypeConfigurationProvider : IEntityTypeConfigurationProvider
    {
        private readonly FeatureBasedTenantInfo _tenantInfo;

        public UserEntityTypeConfigurationProvider(FeatureBasedTenantInfo tenantInfo)
        {
            this._tenantInfo = tenantInfo;
        }

        public void ApplyConfigurations(ModelBuilder modelBuilder)
        {
            var users = modelBuilder.Entity<User>();
            users.ToTable("User", "Identity");
            users.HasKey(user => user.Id);
            users.Ignore(user => user.DomainEvents);
            users.Ignore(user => user.IntegrationEvents);
            users.Property(user => user.Id).ValueGeneratedNever();
            users.Metadata.AddAnnotation("MULTI_TENANT", null);
            users.Property<Guid>("TenantId");
            users.HasQueryFilter(user => EF.Property<Guid>(user, "TenantId") == Guid.Parse(this._tenantInfo.Id));

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
            }).UsePropertyAccessMode(PropertyAccessMode.Field);

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
            }).UsePropertyAccessMode(PropertyAccessMode.Field);

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
            }).UsePropertyAccessMode(PropertyAccessMode.Field);

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
            }).UsePropertyAccessMode(PropertyAccessMode.Field);

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
            }).UsePropertyAccessMode(PropertyAccessMode.Field);

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
            }).UsePropertyAccessMode(PropertyAccessMode.Field);

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
            }).UsePropertyAccessMode(PropertyAccessMode.Field);

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
            }).UsePropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}