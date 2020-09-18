// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Finbuckle.MultiTenant;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Settings;
using Initium.Portal.Queries;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Web.ODataEndpoints.SystemAlert;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Options;
using Microsoft.OData.Edm;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Web.ODataEndpoints.SystemAlert
{
    public class SystemAlertODataControllerTests : BaseODataControllerTest<Portal.Queries.Entities.SystemAlert>
    {
        protected override IEdmModel EdmModel
        {
            get
            {
                var modelBuilder = new ODataConventionModelBuilder(this.Provider);
                var entitySet = modelBuilder.EntitySet<Portal.Queries.Entities.SystemAlert>("SystemAlert");
                entitySet.EntityType.HasKey(entity => entity.Id);
                return modelBuilder.GetEdmModel();
            }
        }

        [Fact]
        public void Filtered_GivenFilterIsNull_ExpectUnfiltered()
        {
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new Mock<ITenantInfo>();
            tenantInfo.Setup(x => x.Id).Returns(TestVariables.TenantId.ToString);

            using var context = new QueryContext(options, tenantInfo.Object);
            context.Add(new Portal.Queries.Entities.SystemAlert
            {
                Id = Guid.NewGuid(),
                Name = "name-1",
                Type = SystemAlertType.Critical,
                Message = "message-1",
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.SystemAlert
            {
                Id = Guid.NewGuid(),
                Name = "name-2",
                Type = SystemAlertType.Critical,
                Message = "message-2",
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.SystemAlert
            {
                Id = Guid.NewGuid(),
                Name = "name-3",
                Type = SystemAlertType.Critical,
                Message = "message-3",
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.SystemAlert
            {
                Id = Guid.NewGuid(),
                Name = "name-4",
                Type = SystemAlertType.Critical,
                Message = "message-4",
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.SaveChanges();

            var systemAlertQueryService = new Mock<ISystemAlertQueryService>();
            systemAlertQueryService.Setup(x => x.QueryableEntity).Returns(context.SystemAlerts);
            var systemAlertController = new SystemAlertODataController(systemAlertQueryService.Object);
            var result =
                Assert.IsType<OkObjectResult>(systemAlertController.Filtered(this.GenerateEmptyQueryOptions(), null));
            var data = Assert.IsType<InternalDbSet<Portal.Queries.Entities.SystemAlert>>(result.Value);
            Assert.Equal(4, data.AsQueryable().Count());
        }
    }
}