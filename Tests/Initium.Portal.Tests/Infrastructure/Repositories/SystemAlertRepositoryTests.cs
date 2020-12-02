// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using System.Threading.Tasks;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.AggregatesModel.SystemAlertAggregate;
using Initium.Portal.Infrastructure;
using Initium.Portal.Infrastructure.Admin;
using Initium.Portal.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Infrastructure.Repositories
{
    public class SystemAlertRepositoryTests
    {
        [Fact]
        public void UnitOfWork_GivenDataContextSetInConstructor_ExpectSameValud()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var repository = new SystemAlertRepository(context);
            Assert.Equal(context, repository.UnitOfWork);
        }

        [Fact]
        public void Add_GivenArgumentIsNotSystemAlertType_ExpectException()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var repository = new SystemAlertRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => repository.Add(new Mock<ISystemAlert>().Object));
            Assert.Equal("systemAlert", exception.Message);
        }

        [Fact]
        public void Add_GivenArgumentIsSystemAlertType_ExpectReturnedUserToBeIdenticalAsArgument()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var repository = new SystemAlertRepository(context);
            var systemAlert = new SystemAlert(
                TestVariables.SystemAlertId,
                "name",
                "message",
                SystemAlertType.Critical,
                whenToShow: TestVariables.Now.AddDays(-1),
                whenToHide: TestVariables.Now.AddDays(1));
            var returnedSystemAlert = repository.Add(systemAlert);
            Assert.NotNull(returnedSystemAlert);
            Assert.Equal(systemAlert, returnedSystemAlert);
        }

        [Fact]
        public void Add_GivenArgumentIsSystemAlertType_ExpectSystemAlertToBeAddedToContext()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var repository = new SystemAlertRepository(context);
            var systemAlert = new SystemAlert(
                TestVariables.SystemAlertId,
                "name",
                "message",
                SystemAlertType.Critical,
                whenToShow: TestVariables.Now.AddDays(-1),
                whenToHide: TestVariables.Now.AddDays(1));
            repository.Add(systemAlert);
            var userInContext = context.ChangeTracker.Entries<SystemAlert>()
                .FirstOrDefault(x => x.Entity.Id == TestVariables.SystemAlertId);
            Assert.NotNull(userInContext);
            Assert.Equal(EntityState.Added, userInContext.State);
        }

        [Fact]
        public void Delete_GivenArgumentIsNotSystemAlert_ExpectArgumentException()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var repository = new SystemAlertRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => repository.Delete(new Mock<ISystemAlert>().Object));
            Assert.Equal("systemAlert", exception.Message);
        }

        [Fact]
        public void Delete_GivenArgumentIsSystemAlert_ExpectSystemAlertToBeDeletedInTheContext()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var repository = new SystemAlertRepository(context);
            var systemAlert = new SystemAlert(
                TestVariables.SystemAlertId,
                "name",
                "message",
                SystemAlertType.Critical,
                whenToShow: TestVariables.Now.AddDays(-1),
                whenToHide: TestVariables.Now.AddDays(1));
            repository.Delete(systemAlert);
            var roleInContext = context.ChangeTracker.Entries<SystemAlert>()
                .FirstOrDefault(x => x.Entity.Id == TestVariables.SystemAlertId);
            Assert.NotNull(roleInContext);
            Assert.Equal(EntityState.Deleted, roleInContext.State);
        }

        [Fact]
        public async Task Find_GivenSystemAlertDoesExist_ExpectMaybeWithData()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new FeatureBasedTenantInfo
            {
                Id = TestVariables.TenantId.ToString(),
            };

            await using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), tenantInfo);
            await context.SystemAlerts.AddAsync(new SystemAlert(
                TestVariables.SystemAlertId,
                "name",
                "message",
                SystemAlertType.Critical,
                whenToShow: TestVariables.Now.AddDays(-1),
                whenToHide: TestVariables.Now.AddDays(1)));
            await context.SaveChangesAsync();
            var repository = new SystemAlertRepository(context);
            var maybe = await repository.Find(TestVariables.SystemAlertId);

            Assert.True(maybe.HasValue);
        }

        [Fact]
        public async Task Find_GivenSystemAlertDoesNotExist_ExpectMaybeWithNoValue()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new FeatureBasedTenantInfo
            {
                Id = TestVariables.TenantId.ToString(),
            };

            await using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), tenantInfo);
            var repository = new SystemAlertRepository(context);
            var maybe = await repository.Find(Guid.Empty);
            Assert.True(maybe.HasNoValue);
        }

        [Fact]
        public void Update_GivenArgumentIsNotRole_ExpectArgumentException()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var repository = new SystemAlertRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => repository.Update(new Mock<ISystemAlert>().Object));
            Assert.Equal("systemAlert", exception.Message);
        }

        [Fact]
        public void Update_GivenArgumentIsRole_ExpectSystemAlertToBeDeletedInTheContext()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var repository = new SystemAlertRepository(context);
            var systemAlert = new SystemAlert(
                TestVariables.SystemAlertId,
                "name",
                "message",
                SystemAlertType.Critical,
                whenToShow: TestVariables.Now.AddDays(-1),
                whenToHide: TestVariables.Now.AddDays(1));
            repository.Update(systemAlert);
            var roleInContext = context.ChangeTracker.Entries<SystemAlert>()
                .FirstOrDefault(x => x.Entity.Id == TestVariables.SystemAlertId);
            Assert.NotNull(roleInContext);
            Assert.Equal(EntityState.Modified, roleInContext.State);
        }
    }
}