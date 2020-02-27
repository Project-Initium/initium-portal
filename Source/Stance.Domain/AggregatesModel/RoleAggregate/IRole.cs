// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Stance.Core.Contracts.Domain;

namespace Stance.Domain.AggregatesModel.RoleAggregate
{
    public interface IRole : IAggregateRoot, IEntity
    {
        string Name { get; }

        IReadOnlyList<RoleResource> RoleResources { get; }

        void UpdateName(string name);

        void SetResources(IReadOnlyList<Guid> resources);
    }
}