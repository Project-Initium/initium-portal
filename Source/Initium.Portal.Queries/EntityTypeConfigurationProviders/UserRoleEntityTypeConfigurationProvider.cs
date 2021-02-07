using System;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries.EntityTypeConfigurationProviders
{
    public class UserRoleEntityTypeConfigurationProvider : IEntityTypeConfigurationProvider
    {
        private readonly FeatureBasedTenantInfo _tenantInfo;

        public UserRoleEntityTypeConfigurationProvider(FeatureBasedTenantInfo tenantInfo)
        {
            this._tenantInfo = tenantInfo;
        }

        public void ApplyConfigurations(ModelBuilder modelBuilder)
        {
            var userRoles = modelBuilder.Entity<UserRoleReadEntity>();

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