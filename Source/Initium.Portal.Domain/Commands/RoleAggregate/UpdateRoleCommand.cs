// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Domain;
using MediatR;
using ResultMonad;

namespace Initium.Portal.Domain.Commands.RoleAggregate
{
    public class UpdateRoleCommand : IRequest<ResultWithError<ErrorData>>
    {
        public UpdateRoleCommand(Guid roleId, string name, List<Guid> resources)
        {
            this.Resources = resources;
            this.RoleId = roleId;
            this.Name = name;
        }

        public Guid RoleId { get; }

        public string Name { get; }

        public IReadOnlyList<Guid> Resources { get; }
    }
}