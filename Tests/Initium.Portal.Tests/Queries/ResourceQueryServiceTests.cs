// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Initium.Portal.Queries;
using Initium.Portal.Queries.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Initium.Portal.Tests.Queries
{
    public class ResourceQueryServiceTests
    {
        [Fact]
        public async Task GetNestedSimpleResources_GivenDataIsFound_ExpectMaybeWithMappedData()
        {
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);
            context.Add(new Resource
            {
                Id = TestVariables.ResourceId,
                Name = "parent-1",
            });
            context.Add(new Resource
            {
                Id = Guid.NewGuid(),
                Name = "parent-2",
            });
            var childId = Guid.NewGuid();
            context.Add(new Resource
            {
                Id = childId,
                Name = "child-1",
                ParentResourceId = TestVariables.ResourceId,
            });
            context.SaveChanges();

            var roleQueries = new ResourceQueryService(context);
            var result = await roleQueries.GetNestedSimpleResources();
            Assert.True(result.HasValue);
            Assert.Equal(2, result.Value.Count);
            var parent1 = result.Value.FirstOrDefault(x => x.Name == "parent-1");
            Assert.NotNull(parent1);
            Assert.Equal("parent-1", parent1.Name);
            Assert.Equal(TestVariables.ResourceId, parent1.Id);
            Assert.Single(parent1.SimpleResources);
            Assert.Equal("child-1", parent1.SimpleResources.First().Name);
            Assert.Equal(childId, parent1.SimpleResources.First().Id);
        }

        [Fact]
        public async Task GetNestedSimpleResources_GivenNoDataIsFound_ExpectMaybeWithNothing()
        {
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            await using var context = new QueryContext(options);

            var roleQueries = new ResourceQueryService(context);
            var result = await roleQueries.GetNestedSimpleResources();
            Assert.True(result.HasNoValue);
        }
    }
}