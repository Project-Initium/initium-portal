// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using Finbuckle.MultiTenant;
using Initium.Portal.Core.Constants;

namespace Initium.Portal.Core.MultiTenant
{
    public class FeatureBasedTenantInfo :
        ITenantInfo
    {
        public List<SystemFeatures> Features { get; set; } = new List<SystemFeatures>();

        public string Id { get; set; }

        public string Identifier { get; set; }

        public string Name { get; set; }

        public string ConnectionString { get; set; }
    }
}