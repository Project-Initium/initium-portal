﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Net;
using Initium.Portal.Web.Infrastructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Initium.Portal.Web.Infrastructure.Attributes
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
                context.Result = new StatusCodeResult((int)HttpStatusCode.Unauthorized);
                return;
            }

            if (user.HasPermissions(this._resource))
            {
                return;
            }

            context.Result = new StatusCodeResult((int)HttpStatusCode.Forbidden);
        }
    }
}