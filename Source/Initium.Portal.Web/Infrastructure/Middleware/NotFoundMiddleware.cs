// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Initium.Portal.Web.Infrastructure.Middleware
{
    public class NotFoundMiddleware
    {
        private readonly RequestDelegate _next;

        public NotFoundMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await this._next(context);

            if (context.Response.StatusCode == 404 && !context.Response.HasStarted)
            {
                context.Request.Path = "/not-found";
                await this._next(context);
            }
        }
    }
}