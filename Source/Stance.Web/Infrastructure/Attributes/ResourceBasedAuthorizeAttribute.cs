// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Stance.Web.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ResourceBasedAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _resource;

        public ResourceBasedAuthorizeAttribute(string resource)
        {
            this._resource = resource;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                return;
            }

            if (user.HasClaim(x => x.Type == ClaimTypes.System))
            {
                return;
            }

            if (user.HasClaim(ClaimTypes.Role, this._resource))
            {
                return;
            }

            context.Result = new StatusCodeResult((int) HttpStatusCode.Forbidden);
        }
    }
}