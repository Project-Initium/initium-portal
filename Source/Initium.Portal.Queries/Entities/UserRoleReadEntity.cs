// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Queries.Entities
{
    public class UserRoleReadEntity
    {
        public Guid RoleId { get; private set; }

        public Guid UserId { get; private set; }

        public RoleReadEntity Role { get; private set; }

        public UserReadEntity User { get; private set; }
    }
}