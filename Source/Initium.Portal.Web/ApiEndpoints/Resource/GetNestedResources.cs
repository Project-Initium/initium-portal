// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.ApiEndpoints;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models.Resource;
using Initium.Portal.Web.Infrastructure.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.ApiEndpoints.Resource
{
    public class GetNestedResources : BaseAsyncEndpoint<List<SimpleResourceModel>>
    {
        private readonly IResourceQueryService _resourceQueryService;

        public GetNestedResources(IResourceQueryService resourceQueryService)
        {
            this._resourceQueryService = resourceQueryService ?? throw new ArgumentNullException(nameof(resourceQueryService));
        }

        [ResourceBasedAuthorize("resource-list")]
        [HttpGet("api/resources/list-nested", Name = "GetNestedResourcesEndpoint")]

        public override async Task<ActionResult<List<SimpleResourceModel>>> HandleAsync(CancellationToken cancellationToken = default)
        {
            var maybe = await this._resourceQueryService.GetNestedSimpleResources();
            return maybe.HasNoValue ? this.Ok(new List<SimpleResourceModel>()) : this.Ok(maybe.Value);
        }
    }
}