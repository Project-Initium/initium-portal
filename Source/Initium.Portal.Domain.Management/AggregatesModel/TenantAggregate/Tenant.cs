// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Initium.Portal.Core.Domain;

namespace Initium.Portal.Common.Domain.AggregatesModel.TenantAggregate
{
    public sealed class Tenant : Entity, ITenant
    {
        private readonly List<TenantFeature> _tenantFeatures;

        public Tenant(Guid id, string identifier, string name, string connectionString)
        {
            this.Id = id;
            this.Identifier = identifier;
            this.Name = name;
            this.ConnectionString = connectionString;
            this._tenantFeatures = new List<TenantFeature>();
        }

        private Tenant()
        {
            this._tenantFeatures = new List<TenantFeature>();
        }

        public string Identifier { get; private set; }

        public string Name { get; private set; }

        public string ConnectionString { get; private set; }

        public DateTime? WhenDisabled { get; private set; }

        public IReadOnlyList<TenantFeature> TenantFeatures => this._tenantFeatures;

        public void UpdateDetails(string identifier, string name, string connectionString)
        {
            this.Identifier = identifier;
            this.Name = name;
            this.ConnectionString = connectionString;
        }

        public void SetTenantFeatures(IReadOnlyList<Guid> tenantFeatures)
        {
            var distinct = tenantFeatures.Distinct().ToList();
            var current = this._tenantFeatures.Select(x => x.Id).ToList();
            var toAdd = distinct.Except(current);
            var toRemove = current.Except(distinct);

            foreach (var item in toRemove)
            {
                this._tenantFeatures.Remove(this._tenantFeatures.Single(x => x.Id == item));
            }

            this._tenantFeatures.AddRange(toAdd.Select(x => new TenantFeature(x)));
        }

        public void Enable()
        {
            this.WhenDisabled = null;
        }

        public void Disable(DateTime whenDisabled)
        {
            this.WhenDisabled = whenDisabled;
        }
    }
}