// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Initium.Portal.Core.Constants;
using Initium.Portal.Queries.Dynamic;
using Initium.Portal.Web.Controllers.OData.SystemAlert;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Xunit;

namespace Initium.Portal.Tests.Web.Controllers.OData.SystemAlert
{
    public class SystemAlertControllerTests
    {
        [Fact]
        public void Filtered_GivenFilterIsNull_ExpectUnfiltered()
        {
            var options = new DbContextOptionsBuilder<ODataContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ODataContext(options);
            context.Add(new Portal.Queries.Dynamic.Entities.SystemAlert
            {
                Id = Guid.NewGuid(),
                Name = "name-1",
                Type = SystemAlertType.Critical,
                Message = "message-1",
            });
            context.Add(new Portal.Queries.Dynamic.Entities.SystemAlert
            {
                Id = Guid.NewGuid(),
                Name = "name-2",
                Type = SystemAlertType.Critical,
                Message = "message-2",
            });
            context.Add(new Portal.Queries.Dynamic.Entities.SystemAlert
            {
                Id = Guid.NewGuid(),
                Name = "name-3",
                Type = SystemAlertType.Critical,
                Message = "message-3",
            });
            context.Add(new Portal.Queries.Dynamic.Entities.SystemAlert
            {
                Id = Guid.NewGuid(),
                Name = "name-4",
                Type = SystemAlertType.Critical,
                Message = "message-4",
            });
            context.SaveChanges();

            var roleController = new SystemAlertController(context);
            var result = Assert.IsType<OkObjectResult>(roleController.Filtered(null));
            var data = Assert.IsType<InternalDbSet<Portal.Queries.Dynamic.Entities.SystemAlert>>(result.Value);
            Assert.Equal(4, data.AsQueryable().Count());
        }
    }
}