// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using MaybeMonad;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static.Models.Role;
using Stance.Web.Controllers.Api.Resource;
using Xunit;

namespace Stance.Tests.Web.Controllers.Api.Resource
{
    public class ResourceApiControllerTests
    {
        [Fact]
        public async Task GetNestResources_GivenNoResources_ExpectJsonWithEmptyList()
        {
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.GetNestedSimpleResources()).ReturnsAsync(Maybe<List<SimpleResourceModel>>.Nothing);
            var controller = new ResourceApiController(roleQueries.Object);

            var response = Assert.IsType<JsonResult>(await controller.GetNestResources());
            var data = Assert.IsType<List<SimpleResourceModel>>(response.Value);
            Assert.Empty(data);
        }

        [Fact]
        public async Task GetNestResources_GivenResourcesAreFound_ExpectJsonWithPopulatedList()
        {
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.GetNestedSimpleResources()).ReturnsAsync(Maybe.From(new List<SimpleResourceModel>
            {
                new SimpleResourceModel(TestVariables.ResourceId, "name", null),
            }));
            var controller = new ResourceApiController(roleQueries.Object);

            var response = Assert.IsType<JsonResult>(await controller.GetNestResources());
            var data = Assert.IsType<List<SimpleResourceModel>>(response.Value);
            Assert.Single(data);
        }
    }
}