// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Finbuckle.MultiTenant;
using Initium.Portal.Queries;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Web.ODataEndpoints.User;
using Microsoft.AspNet.OData.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.OData.Edm;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Web.ODataEndpoints.User
{
    public class UserODataControllerTests : BaseODataControllerTest<Portal.Queries.Entities.User>
    {
        public static IEnumerable<object[]> FilterData
        {
            get
            {
                return new[]
                {
                    new object[] { new UserFilter { Verified = true }, 3 },
                    new object[] { new UserFilter { Verified = true, Unverified = true }, 6 },
                    new object[] { new UserFilter { Unverified = true }, 3 },
                    new object[] { new UserFilter { Locked = true }, 4 },
                    new object[] { new UserFilter { Locked = true, Unlocked = true }, 6 },
                    new object[] { new UserFilter { Unlocked = true }, 2 },
                    new object[] { new UserFilter { Admin = true }, 3 },
                    new object[] { new UserFilter { Admin = true, NonAdmin = true }, 6 },
                    new object[] { new UserFilter { NonAdmin = true }, 3 },
                };
            }
        }

        protected override IEdmModel EdmModel
        {
            get
            {
                var modelBuilder = new ODataConventionModelBuilder(this.Provider);
                var entitySet = modelBuilder.EntitySet<Portal.Queries.Entities.User>("User");
                entitySet.EntityType.HasKey(entity => entity.Id);
                return modelBuilder.GetEdmModel();
            }
        }

        [Theory]
        [MemberData(nameof(FilterData))]
        public void Filtered_GivenFilterIsNotNull_ExpectFilteredData(UserFilter filter, int count)
        {
            var options = new DbContextOptionsBuilder<QueryContext>()
                .UseInMemoryDatabase($"ODataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new Mock<ITenantInfo>();
            tenantInfo.Setup(x => x.Id).Returns(TestVariables.TenantId.ToString);

            using var context = new QueryContext(options, tenantInfo.Object);
            context.Add(new Portal.Queries.Entities.User
            {
                Id = Guid.NewGuid(),
                IsAdmin = false,
                IsLocked = true,
                IsVerified = true,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.User
            {
                Id = Guid.NewGuid(),
                IsAdmin = true,
                IsLocked = false,
                IsVerified = true,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.User
            {
                Id = Guid.NewGuid(),
                IsAdmin = true,
                IsLocked = true,
                IsVerified = false,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.User
            {
                Id = Guid.NewGuid(),
                IsAdmin = false,
                IsLocked = true,
                IsVerified = false,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.User
            {
                Id = Guid.NewGuid(),
                IsAdmin = true,
                IsLocked = true,
                IsVerified = true,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.User
            {
                Id = Guid.NewGuid(),
                IsAdmin = false,
                IsLocked = false,
                IsVerified = false,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.SaveChanges();
            var userQueryService = new Mock<IUserQueryService>();
            userQueryService.Setup(x => x.QueryableEntity).Returns(context.Users);
            var userController = new UserODataController(userQueryService.Object);
            var result = Assert.IsType<OkObjectResult>(userController.Filtered(this.GenerateEmptyQueryOptions(), filter));
            var data = Assert.IsType<EntityQueryable<Portal.Queries.Entities.User>>(result.Value);
            Assert.Equal(count, data.Count());
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
            context.Add(new Portal.Queries.Entities.User
            {
                Id = Guid.NewGuid(),
                IsAdmin = false,
                IsLocked = true,
                IsVerified = true,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.User
            {
                Id = Guid.NewGuid(),
                IsAdmin = true,
                IsLocked = false,
                IsVerified = true,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.User
            {
                Id = Guid.NewGuid(),
                IsAdmin = true,
                IsLocked = true,
                IsVerified = false,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.User
            {
                Id = Guid.NewGuid(),
                IsAdmin = false,
                IsLocked = true,
                IsVerified = false,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.User
            {
                Id = Guid.NewGuid(),
                IsAdmin = true,
                IsLocked = true,
                IsVerified = true,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.Add(new Portal.Queries.Entities.User
            {
                Id = Guid.NewGuid(),
                IsAdmin = false,
                IsLocked = false,
                IsVerified = false,
            }).Property("TenantId").CurrentValue = TestVariables.TenantId;
            context.SaveChanges();

            var userQueryService = new Mock<IUserQueryService>();
            userQueryService.Setup(x => x.QueryableEntity).Returns(context.Users);
            var userController = new UserODataController(userQueryService.Object);
            var result = Assert.IsType<OkObjectResult>(userController.Filtered(this.GenerateEmptyQueryOptions(), null));
            var data = Assert.IsType<InternalDbSet<Portal.Queries.Entities.User>>(result.Value);
            Assert.Equal(6, data.AsQueryable().Count());
        }
    }
}