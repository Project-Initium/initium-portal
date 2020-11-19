// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.MultiTenant;

namespace Initium.Portal.Web.Infrastructure.MultiTenant
{
    [Serializable]
    public class CacheableTenantData
    {
        public CacheableTenantData()
        {
        }

        public CacheableTenantData(FeatureBasedTenantInfo tenantInfo)
        {
            this.Id = tenantInfo.Id;
            this.Identifier = tenantInfo.Identifier;
            this.Name = tenantInfo.Name;
            this.ConnectionString = tenantInfo.ConnectionString;
            this.Features = tenantInfo.Features;
        }

        public IReadOnlyList<SystemFeatures> Features { get; }

        public string Id { get; }

        public string Identifier { get; }

        public string Name { get; }

        public string ConnectionString { get; }

        public bool IsSetup => !string.IsNullOrEmpty(this.Id);
    }
}