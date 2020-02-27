// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using MediatR;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.CommandResults.RoleAggregate;

namespace Stance.Domain.Commands.RoleAggregate
{
    public class CreateRoleCommand : IRequest<Result<CreateRoleCommandResult, ErrorData>>
    {
        private readonly List<Guid> _resources;

        public CreateRoleCommand(string name, List<Guid> resources)
        {
            this.Name = name;
            this._resources = resources;
        }

        public string Name { get; }

        public IReadOnlyList<Guid> Resources => this._resources.AsReadOnly();
    }
}