// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;

namespace Initium.Portal.Domain.CommandResults.RoleAggregate
{
    public class CreateRoleCommandResult
    {
        public CreateRoleCommandResult(Guid roleId)
        {
            this.RoleId = roleId;
        }

        public Guid RoleId { get; }
    }
}