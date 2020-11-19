// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Core.Settings;
using Microsoft.Extensions.Options;

namespace Initium.Portal.Web.Infrastructure.MultiTenant
{
    public class ManagementMultiTenantStore : IMultiTenantStore<FeatureBasedTenantInfo>
    {
        private readonly MultiTenantSettings _multiTenantSettings;

        public ManagementMultiTenantStore(IOptions<MultiTenantSettings> multiTenantSettings)
        {
            if (multiTenantSettings == null)
            {
                throw new ArgumentNullException(nameof(multiTenantSettings));
            }

            this._multiTenantSettings = multiTenantSettings.Value;
        }

        public Task<bool> TryAddAsync(FeatureBasedTenantInfo tenantInfo)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> TryUpdateAsync(FeatureBasedTenantInfo tenantInfo)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> TryRemoveAsync(string id)
        {
            throw new System.NotImplementedException();
        }

        public Task<FeatureBasedTenantInfo> TryGetByIdentifierAsync(string identifier)
        {
            return Task.FromResult(new FeatureBasedTenantInfo
            {
                Id = this._multiTenantSettings.DefaultTenantId.ToString(),
                Identifier = this._multiTenantSettings.DefaultIdentifier,
                Name = this._multiTenantSettings.DefaultName,
                ConnectionString = this._multiTenantSettings.DefaultTenantConnectionString,
            });
        }

        public Task<FeatureBasedTenantInfo> TryGetAsync(string id)
        {
            return Task.FromResult(new FeatureBasedTenantInfo
            {
                Id = this._multiTenantSettings.DefaultTenantId.ToString(),
                Identifier = this._multiTenantSettings.DefaultIdentifier,
                Name = this._multiTenantSettings.DefaultName,
                ConnectionString = this._multiTenantSettings.DefaultTenantConnectionString,
            });
        }

        public Task<IEnumerable<FeatureBasedTenantInfo>> GetAllAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}