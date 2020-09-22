// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Finbuckle.MultiTenant;
using Initium.Portal.Core.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Initium.Portal.Web.Infrastructure.Extensions
{
    public static class HttpContextExtensions
    {
        public static bool IsSystemOwner(this HttpContext context)
        {
            var tenantContext = context.GetMultiTenantContext<TenantInfo>();
            var multiTenantSettings = context.RequestServices.GetRequiredService<IOptions<MultiTenantSettings>>();
            return tenantContext.TenantInfo.Id.Equals(
                multiTenantSettings.Value.DefaultTenantId.ToString(),
                StringComparison.InvariantCultureIgnoreCase);
        }
    }
}