// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.MultiTenant;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;

namespace Initium.Portal.Web.Tenant.Infrastructure.FeatureManagement
{
    [FilterAlias("Tenant")]
    public class TenantFeatureFilter : IFeatureFilter
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TenantFeatureFilter(IHttpContextAccessor httpContextAccessor)
        {
            this._httpContextAccessor = httpContextAccessor;
        }

        public Task<bool> EvaluateAsync(FeatureFilterEvaluationContext context)
        {
            var settings = context.Parameters.Get<TenantFeatureFilterSettings>();
            if (!settings.ManagementEnabled)
            {
                return Task.FromResult(false);
            }

            var scope = this._httpContextAccessor.HttpContext.RequestServices.CreateScope();
            var tenantInfo = scope.ServiceProvider.GetService<FeatureBasedTenantInfo>();

            if (tenantInfo == null)
            {
                return Task.FromResult(false);
            }

            var systemFeature = Enum.Parse<SystemFeatures>(context.FeatureName);
            return Task.FromResult(tenantInfo.Features.Contains(systemFeature));
        }
    }
}