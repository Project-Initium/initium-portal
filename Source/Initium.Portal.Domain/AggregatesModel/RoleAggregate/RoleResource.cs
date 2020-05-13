// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Domain;

namespace Initium.Portal.Domain.AggregatesModel.RoleAggregate
{
    public sealed class RoleResource : Entity
    {
        public RoleResource(Guid id)
        {
            this.Id = id;
        }

        private RoleResource()
        {
        }
    }
}