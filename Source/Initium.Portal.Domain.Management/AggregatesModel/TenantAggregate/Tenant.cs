// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Domain;
using Newtonsoft.Json;

namespace Initium.Portal.Common.Domain.AggregatesModel.TenantAggregate
{
    public sealed class Tenant : Entity, ITenant
    {
        public Tenant(Guid id, string identifier, string name, string connectionString)
        {
            this.Id = id;
            this.Identifier = identifier;
            this.Name = name;
            this.ConnectionString = connectionString;
        }

        private Tenant()
        {
        }

        public string Identifier { get; private set; }

        public string Name { get; private set; }

        public string ConnectionString { get; private set; }

        public DateTime? WhenDisabled { get; private set; }

        public string SystemFeaturesJson { get; private set; }

        public IReadOnlyList<SystemFeatures> SystemFeatures
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.SystemFeaturesJson))
                {
                    return new List<SystemFeatures>();
                }

                var data = JsonConvert.DeserializeObject<List<SystemFeatures>>(this.SystemFeaturesJson);
                return data ?? new List<SystemFeatures>();
            }
        }

        public void UpdateDetails(string identifier, string name, string connectionString)
        {
            this.Identifier = identifier;
            this.Name = name;
            this.ConnectionString = connectionString;
        }

        public void SetSystemFeatures(IReadOnlyList<SystemFeatures> tenantFeatures)
        {
            var currentTenantFeatures = this.SystemFeatures.ToList();
            var distinct = tenantFeatures.Distinct().ToList();
            var current = currentTenantFeatures.Select(x => x).ToList();
            var toAdd = distinct.Except(current);
            var toRemove = current.Except(distinct);

            foreach (var item in toRemove)
            {
                currentTenantFeatures.Remove(item);
            }

            currentTenantFeatures.AddRange(toAdd);

            this.SystemFeaturesJson = JsonConvert.SerializeObject(currentTenantFeatures);
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