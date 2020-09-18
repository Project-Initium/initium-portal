// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using Finbuckle.MultiTenant;
using Initium.Portal.Core.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Initium.Portal.Web.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class SystemOwnerAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var tenantInfo = context.HttpContext.RequestServices.GetRequiredService<ITenantInfo>();
            var multiTenantSettings = context.HttpContext.RequestServices.GetRequiredService<IOptions<MultiTenantSettings>>();

            if (tenantInfo.Id.Equals(
                multiTenantSettings.Value.DefaultTenantId.ToString(),
                StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            context.Result = new StatusCodeResult((int)HttpStatusCode.NotFound);
        }
    }
}