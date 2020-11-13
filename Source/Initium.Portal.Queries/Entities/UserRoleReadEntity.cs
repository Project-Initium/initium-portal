// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Queries.Entities
{
    public class UserRoleReadEntity
    {
        public Guid RoleId { get; set; }

        public Guid UserId { get; set; }

        public RoleReadEntity Role { get; set; }

        public UserReadEntity User { get; set; }
    }
}