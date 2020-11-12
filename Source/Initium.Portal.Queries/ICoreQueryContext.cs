// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;

namespace Initium.Portal.Queries
{
    public interface ICoreQueryContext
    {
        DbSet<User> Users { get; set; }

        DbSet<Role> Roles { get; set; }

        DbSet<UserNotification> UserNotifications { get; set; }

        DbSet<SystemAlert> SystemAlerts { get; set; }

        DbSet<Resource> Resources { get; set; }

        DbSet<RoleResource> RoleResources { get; set; }

        DbSet<UserRole> UserRoles { get; set; }
    }
}