// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models.Resource;
using Initium.Portal.Web.ApiEndpoints.Resource;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Web.ApiEndpoints.Resource
{
    public class GetNestedResourcesTests
    {
        [Fact]
        public async Task HandleAsync_GivenNoResources_ExpectJsonWithEmptyList()
        {
            var roleQueries = new Mock<IResourceQueryService>();
            roleQueries.Setup(x => x.GetFeatureBasedNestedSimpleResources(It.IsAny<CancellationToken>())).ReturnsAsync(new List<SimpleResourceModel>());
            var endpoint = new GetNestedResources(roleQueries.Object);

            var response = await endpoint.HandleAsync();
            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<List<SimpleResourceModel>>(rawResult.Value);
            Assert.Empty(result);
        }

        [Fact]
        public async Task HandleAsync_GivenResourcesAreFound_ExpectJsonWithPopulatedList()
        {
            var roleQueries = new Mock<IResourceQueryService>();
            roleQueries.Setup(x => x.GetFeatureBasedNestedSimpleResources(It.IsAny<CancellationToken>())).ReturnsAsync(new List<SimpleResourceModel>
            {
                new SimpleResourceModel(TestVariables.ResourceId, "name", null, null),
            });
            var endpoint = new GetNestedResources(roleQueries.Object);

            var response = await endpoint.HandleAsync();
            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<List<SimpleResourceModel>>(rawResult.Value);
            Assert.Single(result);
        }
    }
}