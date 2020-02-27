// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using MediatR;
using ResultMonad;
using Stance.Core.Domain;

namespace Stance.Domain.Commands.RoleAggregate
{
    public class UpdateRoleCommand : IRequest<ResultWithError<ErrorData>>
    {
        private readonly List<Guid> _resources;

        public UpdateRoleCommand(Guid roleId, string name, List<Guid> resources)
        {
            this._resources = resources;
            this.RoleId = roleId;
            this.Name = name;
        }

        public Guid RoleId { get; }

        public string Name { get; }

        public IReadOnlyList<Guid> Resources => this._resources.AsReadOnly();
    }
}