// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
using Initium.Portal.Infrastructure;
using Initium.Portal.Infrastructure.Admin;
using Initium.Portal.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Infrastructure.Repositories
{
    public class RoleRepositoryTests
    {
        [Fact]
        public void Add_GivenArgumentIsNotRole_ExpectArgumentException()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var roleRepository = new RoleRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => roleRepository.Add(new Mock<IRole>().Object));
            Assert.Equal("role", exception.Message);
        }

        [Fact]
        public void Add_GivenArgumentIsRole_ExpectRoleToBeAddedToTheContext()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var roleRepository = new RoleRepository(context);
            var role = new Role(TestVariables.RoleId, "name", new List<Guid>());
            roleRepository.Add(role);
            var roleInContext = context.ChangeTracker.Entries<Role>().FirstOrDefault(x => x.Entity.Id == TestVariables.RoleId);
            Assert.NotNull(roleInContext);
            Assert.Equal(EntityState.Added, roleInContext.State);
        }

        [Fact]
        public void Delete_GivenArgumentIsNotRole_ExpectArgumentException()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var roleRepository = new RoleRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => roleRepository.Delete(new Mock<IRole>().Object));
            Assert.Equal("role", exception.Message);
        }

        [Fact]
        public void Delete_GivenArgumentIsRole_ExpectRoleToBeDeletedInTheContext()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var roleRepository = new RoleRepository(context);
            var role = new Role(TestVariables.RoleId, "nane", new List<Guid>());
            roleRepository.Delete(role);
            var roleInContext = context.ChangeTracker.Entries<Role>().FirstOrDefault(x => x.Entity.Id == TestVariables.RoleId);
            Assert.NotNull(roleInContext);
            Assert.Equal(EntityState.Deleted, roleInContext.State);
        }

        [Fact]
        public async Task Find_GivenUserDoesExist_ExpectMaybeWithUser()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new FeatureBasedTenantInfo
            {
                Id = TestVariables.TenantId.ToString(),
            };

            await using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), tenantInfo);
            var role = new Role(TestVariables.RoleId, "name", new List<Guid>());
            await context.Roles.AddAsync(role);
            await context.SaveEntitiesAsync();
            var roleRepository = new RoleRepository(context);
            var maybe = await roleRepository.Find(TestVariables.RoleId);
            Assert.True(maybe.HasValue);
        }

        [Fact]
        public async Task Find_GivenUserDoesNotExist_ExpectMaybeWithNoData()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new FeatureBasedTenantInfo
            {
                Id = TestVariables.TenantId.ToString(),
            };

            await using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), tenantInfo);
            var roleRepository = new RoleRepository(context);
            var maybe = await roleRepository.Find(TestVariables.RoleId);
            Assert.True(maybe.HasNoValue);
        }

        [Fact]
        public void Update_GivenArgumentIsNotRole_ExpectArgumentException()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var roleRepository = new RoleRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => roleRepository.Update(new Mock<IRole>().Object));
            Assert.Equal("role", exception.Message);
        }

        [Fact]
        public void Update_GivenArgumentIsRole_ExpectRoleToBeUpdatedInTheContext()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var roleRepository = new RoleRepository(context);
            var role = new Role(TestVariables.RoleId, "name", new List<Guid>());
            roleRepository.Update(role);
            var roleInContext = context.ChangeTracker.Entries<Role>().FirstOrDefault(x => x.Entity.Id == TestVariables.RoleId);
            Assert.NotNull(roleInContext);
            Assert.Equal(EntityState.Modified, roleInContext.State);
        }
    }
}