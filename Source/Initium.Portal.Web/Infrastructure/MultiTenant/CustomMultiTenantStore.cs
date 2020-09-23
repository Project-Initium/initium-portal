// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Initium.Portal.Core.Extensions;
using Initium.Portal.Core.Settings;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using NodaTime;

namespace Initium.Portal.Web.Infrastructure.MultiTenant
{
    public class CustomMultiTenantStore : IMultiTenantStore<TenantInfo>
    {
        private readonly MultiTenantSettings _multiTenantSettings;
        private readonly IDistributedCache _distributedCache;
        private readonly IClock _clock;

        public CustomMultiTenantStore(IOptions<MultiTenantSettings> multiTenantSettings, IDistributedCache distributedCache, IClock clock)
        {
            if (multiTenantSettings == null)
            {
                throw new ArgumentNullException(nameof(multiTenantSettings));
            }

            this._distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            this._clock = clock ?? throw new ArgumentNullException(nameof(clock));

            this._multiTenantSettings = multiTenantSettings.Value;
        }

        public async Task<bool> TryAddAsync(TenantInfo tenantInfo)
        {
            await this._distributedCache.AddValue(tenantInfo.Identifier, new CacheableTenantData(tenantInfo), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = this._clock.GetCurrentInstant().ToDateTimeOffset().AddMinutes(30),
            });
            return true;
        }

        public async Task<bool> TryUpdateAsync(TenantInfo tenantInfo)
        {
            return await this.TryAddAsync(tenantInfo);
        }

        public async Task<bool> TryRemoveAsync(string id)
        {
            await this._distributedCache.RemoveAsync(id);
            return true;
        }

        public async Task<TenantInfo> TryGetByIdentifierAsync(string identifier)
        {
            var maybe = await this._distributedCache.TryGetValue<CacheableTenantData>(identifier);
            if (maybe.HasValue)
            {
                if (!maybe.Value.IsSetup)
                {
                    return null;
                }

                return new TenantInfo
                {
                    Id = maybe.Value.Id,
                    Identifier = maybe.Value.Identifier,
                    Name = maybe.Value.Name,
                    ConnectionString = maybe.Value.ConnectionString,
                };
            }

            await using var sql = new SqlConnection(this._multiTenantSettings.DefaultTenantConnectionString);
            await using var cmd = new SqlCommand("[Portal].[uspGetTenantInfoByIdentifier]", sql)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            cmd.Parameters.Add(new SqlParameter("@identifier", identifier));
            await sql.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var tenantInfo = new TenantInfo
                {
                    Id = reader.GetFieldValue<Guid>(0).ToString(),
                    Identifier = reader.GetFieldValue<string>(1),
                    Name = reader.GetFieldValue<string>(2),
                    ConnectionString = reader.GetFieldValue<string>(3),
                };
                await this._distributedCache.AddValue(identifier, new CacheableTenantData(tenantInfo), new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = this._clock.GetCurrentInstant().ToDateTimeOffset().AddMinutes(30),
                });
                return tenantInfo;
            }

            await this._distributedCache.AddValue(identifier, new CacheableTenantData(), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = this._clock.GetCurrentInstant().ToDateTimeOffset().AddMinutes(30),
            });
            return null;
        }

        public async Task<TenantInfo> TryGetAsync(string id)
        {
            if (!Guid.TryParse(id, out var realId))
            {
                return null;
            }

            var maybe = await this._distributedCache.TryGetValue<CacheableTenantData>(id);
            if (maybe.HasValue)
            {
                if (!maybe.Value.IsSetup)
                {
                    return null;
                }

                return new TenantInfo
                {
                    Id = maybe.Value.Id,
                    Identifier = maybe.Value.Identifier,
                    Name = maybe.Value.Name,
                    ConnectionString = maybe.Value.ConnectionString,
                };
            }

            await using var sql = new SqlConnection(this._multiTenantSettings.DefaultTenantConnectionString);
            await using var cmd = new SqlCommand("[Portal].[uspGetTenantInfoByIdentifier]", sql)
            {
                CommandType = System.Data.CommandType.StoredProcedure,
            };
            cmd.Parameters.Add(new SqlParameter("@id", realId));
            await sql.OpenAsync();

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var tenantInfo = new TenantInfo
                {
                    Id = reader.GetFieldValue<Guid>(0).ToString(),
                    Identifier = reader.GetFieldValue<string>(1),
                    Name = reader.GetFieldValue<string>(2),
                    ConnectionString = reader.GetFieldValue<string>(3),
                };
                await this._distributedCache.AddValue(id, new CacheableTenantData(tenantInfo), new DistributedCacheEntryOptions
                {
                    AbsoluteExpiration = this._clock.GetCurrentInstant().ToDateTimeOffset().AddMinutes(30),
                });

                return tenantInfo;
            }

            await this._distributedCache.AddValue(id, new CacheableTenantData(), new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = this._clock.GetCurrentInstant().ToDateTimeOffset().AddMinutes(30),
            });
            return null;
        }

        public Task<IEnumerable<TenantInfo>> GetAllAsync()
        {
            throw new NotImplementedException("This should be perform via user interaction");
        }
    }
}