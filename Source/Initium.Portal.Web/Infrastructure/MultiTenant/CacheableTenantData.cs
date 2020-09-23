// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Finbuckle.MultiTenant;

namespace Initium.Portal.Web.Infrastructure.MultiTenant
{
    [Serializable]
    public class CacheableTenantData
    {
        public CacheableTenantData()
        {
        }

        public CacheableTenantData(ITenantInfo tenantInfo)
        {
            this.Id = tenantInfo.Id;
            this.Identifier = tenantInfo.Identifier;
            this.Name = tenantInfo.Name;
            this.ConnectionString = tenantInfo.ConnectionString;
        }

        public string Id { get; }

        public string Identifier { get; }

        public string Name { get; }

        public string ConnectionString { get; }

        public bool IsSetup => !string.IsNullOrEmpty(this.Id);
    }
}