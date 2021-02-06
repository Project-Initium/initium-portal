// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries
{
    public interface ICoreQueryContext
    {
        DbSet<UserReadEntity> Users { get; set; }

        DbSet<RoleReadEntity> Roles { get; set; }

        DbSet<UserNotificationReadEntity> UserNotifications { get; set; }

        DbSet<SystemAlertReadEntity> SystemAlerts { get; set; }

        DbSet<ResourceReadEntity> Resources { get; set; }

        DbSet<RoleResourceReadEntity> RoleResources { get; set; }

        DbSet<UserRoleReadEntity> UserRoles { get; set; }
    }
}