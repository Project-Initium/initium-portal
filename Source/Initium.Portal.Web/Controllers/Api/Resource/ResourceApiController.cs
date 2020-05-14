// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Initium.Portal.Queries.Contracts.Static;
using Initium.Portal.Queries.Static.Models.Role;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Controllers.Api.Resource
{
    [Authorize]
    public class ResourceApiController : Controller
    {
        private readonly IRoleQueries _roleQueries;

        public ResourceApiController(IRoleQueries roleQueries)
        {
            this._roleQueries = roleQueries;
        }

        [HttpGet("api/resources/list-nested")]
        public async Task<IActionResult> GetNestResources()
        {
            var maybe = await this._roleQueries.GetNestedSimpleResources();
            return maybe.HasNoValue ? this.Json(new List<SimpleResourceModel>()) : this.Json(maybe.Value);
        }
    }
}