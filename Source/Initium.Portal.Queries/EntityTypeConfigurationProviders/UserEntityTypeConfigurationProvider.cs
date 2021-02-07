using System;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries.EntityTypeConfigurationProviders
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
            var users = modelBuilder.Entity<UserReadEntity>();

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
    }
}