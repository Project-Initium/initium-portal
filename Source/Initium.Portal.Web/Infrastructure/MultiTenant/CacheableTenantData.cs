// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.MultiTenant;
using ProtoBuf;

namespace Initium.Portal.Web.Infrastructure.MultiTenant
{
    [ProtoContract(SkipConstructor = true)]
    public class CacheableTenantData
    {
        public CacheableTenantData(FeatureBasedTenantInfo tenantInfo)
        {
            this.Id = tenantInfo.Id;
            this.Identifier = tenantInfo.Identifier;
            this.Name = tenantInfo.Name;
            this.ConnectionString = tenantInfo.ConnectionString;
            this.Features = tenantInfo.Features;
        }

        [ProtoMember(1)]
        public string Id { get; private set; }

        [ProtoMember(2)]
        public string Identifier { get; private set; }

        [ProtoMember(3)]
        public string Name { get; private set; }

        [ProtoMember(4)]
        public string ConnectionString { get; private set; }

        [ProtoMember(5)]
        public IReadOnlyList<SystemFeatures> Features { get; private set; }

        public bool IsSetup => !string.IsNullOrEmpty(this.Id);
    }
}