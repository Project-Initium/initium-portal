// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Domain;

namespace Initium.Portal.Common.Domain.AggregatesModel.TenantAggregate
{
    public sealed class TenantFeature : Entity
    {
        public TenantFeature(Guid id)
        {
            this.Id = id;
        }

        private TenantFeature()
        {
        }
    }
}