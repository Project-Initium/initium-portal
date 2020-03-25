// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static.Models.Role;

namespace Stance.Web.Controllers.Api.Resource
{
    [Authorize]
    public class ResourceController : Controller
    {
        private readonly IRoleQueries _roleQueries;

        public ResourceController(IRoleQueries roleQueries)
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