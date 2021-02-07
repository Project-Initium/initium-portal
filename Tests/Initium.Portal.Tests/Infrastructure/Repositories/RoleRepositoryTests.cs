// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.AggregatesModel.RoleAggregate;
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
        public void UnitOfWork_GivenDataContextSetInConstructor_ExpectSameValud()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var repository = new RoleRepository(context);
            Assert.Equal(context, repository.UnitOfWork);
        }

        [Fact]
        public void Add_GivenArgumentIsNotRole_ExpectArgumentException()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var roleRepository = new RoleRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => roleRepository.Add(new Mock<IRole>().Object));
            Assert.Equal("role", exception.ParamName);
        }

        [Fact]
        public void Add_GivenArgumentIsRole_ExpectRoleToBeAddedToTheContext()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
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
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var roleRepository = new RoleRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => roleRepository.Delete(new Mock<IRole>().Object));
            Assert.Equal("role", exception.ParamName);
        }

        [Fact]
        public void Delete_GivenArgumentIsRole_ExpectRoleToBeDeletedInTheContext()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
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
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .Options;

            await using var context = new GenericDataContext(options, serviceProvider.Object, TestVariables.TenantInfo);
            var role = new Role(TestVariables.RoleId, "name", new List<Guid>());
            await context.Set<Role>().AddAsync(role);
            await context.SaveEntitiesAsync();
            var roleRepository = new RoleRepository(context);
            var maybe = await roleRepository.Find(TestVariables.RoleId);
            Assert.True(maybe.HasValue);
        }

        [Fact]
        public async Task Find_GivenUserDoesNotExist_ExpectMaybeWithNoData()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .Options;

            await using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var roleRepository = new RoleRepository(context);
            var maybe = await roleRepository.Find(TestVariables.RoleId);
            Assert.True(maybe.HasNoValue);
        }

        [Fact]
        public void Update_GivenArgumentIsNotRole_ExpectArgumentException()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var roleRepository = new RoleRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => roleRepository.Update(new Mock<IRole>().Object));
            Assert.Equal("role", exception.ParamName);
        }

        [Fact]
        public void Update_GivenArgumentIsRole_ExpectRoleToBeUpdatedInTheContext()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var roleRepository = new RoleRepository(context);
            var role = new Role(TestVariables.RoleId, "name", new List<Guid>());
            roleRepository.Update(role);
            var roleInContext = context.ChangeTracker.Entries<Role>().FirstOrDefault(x => x.Entity.Id == TestVariables.RoleId);
            Assert.NotNull(roleInContext);
            Assert.Equal(EntityState.Modified, roleInContext.State);
        }
    }
}