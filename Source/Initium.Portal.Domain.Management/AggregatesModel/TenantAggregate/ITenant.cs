// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Contracts.Domain;

namespace Initium.Portal.Common.Domain.AggregatesModel.TenantAggregate
{
    public interface ITenant : IAggregateRoot, IEntity
    {
        string Identifier { get; }

        string Name { get; }

        string ConnectionString { get; }

        DateTime? WhenDisabled { get; }

        IReadOnlyList<TenantFeature> TenantFeatures { get; }

        void UpdateDetails(string identifier, string name, string connectionString);

        void SetTenantFeatures(IReadOnlyList<Guid> tenantFeatures);

        void Enable();

        void Disable(DateTime whenDisabled);
    }
}