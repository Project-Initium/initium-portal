// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models.Resource;
using Initium.Portal.Web.Infrastructure.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Controllers.Api.Resource
{
    public class ResourceApiController : Controller
    {
        private readonly IResourceQueryService _resourceQueryService;

        public ResourceApiController(IResourceQueryService resourceQueryService)
        {
            this._resourceQueryService = resourceQueryService;
        }

        [ResourceBasedAuthorize("resource-list")]
        [HttpGet("api/resources/list-nested")]
        public async Task<IActionResult> GetNestResources()
        {
            var maybe = await this._resourceQueryService.GetNestedSimpleResources();
            return maybe.HasNoValue ? this.Json(new List<SimpleResourceModel>()) : this.Json(maybe.Value);
        }
    }
}