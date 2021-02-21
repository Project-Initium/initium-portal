// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Initium.Portal.Common.Domain.AggregatesModel.TenantAggregate;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Infrastructure.Management.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Infrastructure_Management.Repositories
{
    public class TenantRepositoryTests
    {
        [Fact]
        public void UnitOfWork_GivenDataContextSetInConstructor_ExpectSameValue()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddManagementEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>()
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var repository = new TenantRepository(context);
            Assert.Equal(context, repository.UnitOfWork);
        }

        [Fact]
        public void Add_GivenArgumentIsNotTenantType_ExpectException()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddManagementEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>()
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var repository = new TenantRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => repository.Add(new Mock<ITenant>().Object));
            Assert.Equal("tenant", exception.Message);
        }

        [Fact]
        public void Add_GivenArgumentIsTenantType_ExpectReturnedTenantToBeIdenticalAsArgument()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddManagementEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>()
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var repository = new TenantRepository(context);

            var tenant = new Tenant(TestVariables.TenantId, "identifier", "name", "connection-string");
            var returned = repository.Add(tenant);
            Assert.NotNull(returned);
            Assert.Equal(tenant, returned);
        }

        [Fact]
        public void Add_GivenArgumentIsTenantType_ExpectTenantToBeAddedToContext()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddManagementEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>()
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var repository = new TenantRepository(context);

            var tenant = new Tenant(TestVariables.TenantId, "identifier", "name", "connection-string");
            repository.Add(tenant);
            var inContext = context.ChangeTracker.Entries<Tenant>()
                .FirstOrDefault(x => x.Entity.Id == TestVariables.TenantId);
            Assert.NotNull(inContext);
            Assert.Equal(EntityState.Added, inContext.State);
        }

        [Fact]
        public void Update_GivenArgumentIsNotTenant_ExpectArgumentException()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddManagementEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>()
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var repository = new TenantRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => repository.Update(new Mock<ITenant>().Object));
            Assert.Equal("tenant", exception.Message);
        }

        [Fact]
        public void Update_GivenArgumentIsTenant_ExpectTenantToBeUpdatedInTheContext()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddManagementEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>()
                .Options;

            using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var repository = new TenantRepository(context);
            var tenant = new Tenant(TestVariables.TenantId, "identifier", "name", "connection-string");
            repository.Update(tenant);
            var inContext = context.ChangeTracker.Entries<Tenant>()
                .FirstOrDefault(x => x.Entity.Id == TestVariables.TenantId);
            Assert.NotNull(inContext);
            Assert.Equal(EntityState.Modified, inContext.State);
        }

        [Fact]
        public async Task Find_GivenSystemAlertDoesExist_ExpectMaybeWithData()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddManagementEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>()
                .Options;

            await using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            await context.Set<Tenant>().AddAsync(
                new Tenant(TestVariables.TenantId, "identifier", "name", "connection-string"));
            await context.SaveChangesAsync();
            var repository = new TenantRepository(context);
            var maybe = await repository.Find(TestVariables.TenantId);
            Assert.True(maybe.HasValue);
        }

        [Fact]
        public async Task Find_GivenSystemAlertDoesNotExist_ExpectMaybeWithNoValue()
        {
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddManagementEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var options = new DbContextOptionsBuilder<GenericDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .UseApplicationServiceProvider(serviceProvider.Object)
                .ReplaceService<IModelCacheKeyFactory, NoCacheModelCacheKeyFactory>()
                .Options;

            await using var context = new GenericDataContext(options, serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            var repository = new TenantRepository(context);
            var maybe = await repository.Find(Guid.Empty);
            Assert.True(maybe.HasNoValue);
        }
    }
}