﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Initium.Portal.Infrastructure;
using Initium.Portal.Infrastructure.Admin;
using Initium.Portal.Infrastructure.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Initium.Portal.Tests.Infrastructure.Repositories
{
    public sealed class UserRepositoryTests
    {
        [Fact]
        public void Add_GivenArgumentIsNotUserType_ExpectException()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var userRepository = new UserRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => userRepository.Add(new Mock<IUser>().Object));
            Assert.Equal("user", exception.Message);
        }

        [Fact]
        public void Add_GivenArgumentIsUserType_ExpectReturnedUserToBeIdenticalAsArgument()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var userRepository = new UserRepository(context);
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>
                {
                    TestVariables.RoleId,
                },
                true);
            var returnedUser = userRepository.Add(user);
            Assert.NotNull(returnedUser);
            Assert.Equal(user, returnedUser);
        }

        [Fact]
        public void Add_GivenArgumentIsUserType_ExpectUserToBeAddedToContext()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var userRepository = new UserRepository(context);
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>
                {
                    TestVariables.RoleId,
                },
                true);
            userRepository.Add(user);
            var userInContext = context.ChangeTracker.Entries<User>().FirstOrDefault(x => x.Entity.Id == TestVariables.UserId);
            Assert.NotNull(userInContext);
            Assert.Equal(EntityState.Added, userInContext.State);
        }

        [Fact]
        public async Task Find_GivenUserDoesExist_ExpectMaybeWithData()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new FeatureBasedTenantInfo
            {
                Id = TestVariables.TenantId.ToString(),
            };

            await using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), tenantInfo);
            await context.Users.AddAsync(new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>
                {
                    TestVariables.RoleId,
                },
                true));
            await context.SaveChangesAsync();
            var userRepository = new UserRepository(context);
            var maybe = await userRepository.Find(TestVariables.UserId);

            Assert.True(maybe.HasValue);
        }

        [Fact]
        public async Task Find_GivenUserDoesNotExist_ExpectMaybeWithNoValue()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new FeatureBasedTenantInfo
            {
                Id = TestVariables.TenantId.ToString(),
            };

            await using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), tenantInfo);
            var userRepository = new UserRepository(context);
            var maybe = await userRepository.Find(Guid.Empty);
            Assert.True(maybe.HasNoValue);
        }

        [Fact]
        public void Update_GivenArgumentIsNotUserType_ExpectException()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var userRepository = new UserRepository(context);
            var exception = Assert.Throws<ArgumentException>(() => userRepository.Update(new Mock<IUser>().Object));
            Assert.Equal("user", exception.Message);
        }

        [Fact]
        public void Update_GivenArgumentIsUserType_ExpectUserToBeUpdatedInContext()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), Mock.Of<FeatureBasedTenantInfo>());
            var userRepository = new UserRepository(context);
            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>
                {
                    TestVariables.RoleId,
                },
                true);
            userRepository.Add(user);
            userRepository.Update(user);
            var userInContext = context.ChangeTracker.Entries<User>().FirstOrDefault(x => x.Entity.Id == TestVariables.UserId);
            Assert.NotNull(userInContext);
            Assert.Equal(EntityState.Modified, userInContext.State);
        }

        [Fact]
        public async Task FindByUserBySecurityToken_GivenUserDoesExist_ExpectMaybeWithData()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>
                {
                    TestVariables.RoleId,
                },
                true);
            user.GenerateNewPasswordResetToken(TestVariables.Now, TimeSpan.FromHours(2));
            var tokenId = user.SecurityTokenMappings.First().Id;

            var tenantInfo = new FeatureBasedTenantInfo
            {
                Id = TestVariables.TenantId.ToString(),
            };

            await using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), tenantInfo);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            var userRepository = new UserRepository(context);
            var maybe = await userRepository.FindByUserBySecurityToken(tokenId, TestVariables.Now);

            Assert.True(maybe.HasValue);
        }

        [Fact]
        public async Task FindByUserBySecurityToken_GivenUserDoesNotExist_ExpectMaybeWithNoValue()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var mediator = new Mock<IMediator>();

            var tenantInfo = new FeatureBasedTenantInfo
            {
                Id = TestVariables.TenantId.ToString(),
            };

            await using var context = new ManagementDataContext(options, mediator.Object, tenantInfo);
            var userRepository = new UserRepository(context);
            var maybe = await userRepository.FindByUserBySecurityToken(Guid.Empty, DateTime.UtcNow);
            Assert.True(maybe.HasNoValue);
        }

        [Fact]
        public async Task FindByEmailAddress_GivenUserDoesExist_ExpectMaybeWithData()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var user = new User(
                TestVariables.UserId,
                "email-address",
                "password-hash",
                false,
                TestVariables.Now,
                "first-name",
                "last-name",
                new List<Guid>
                {
                    TestVariables.RoleId,
                },
                true);

            var tenantInfo = new FeatureBasedTenantInfo
            {
                Id = TestVariables.TenantId.ToString(),
            };

            await using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), tenantInfo);
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
            var userRepository = new UserRepository(context);
            var maybe = await userRepository.FindByEmailAddress(new string("email-address"));

            Assert.True(maybe.HasValue);
        }

        [Fact]
        public async Task FindByEmailAddress_GivenUserDoesNotExist_ExpectMaybeWithNoValue()
        {
            var options = new DbContextOptionsBuilder<CoreDataContext>()
                .UseInMemoryDatabase($"DataContext{Guid.NewGuid()}")
                .Options;

            var tenantInfo = new FeatureBasedTenantInfo
            {
                Id = TestVariables.TenantId.ToString(),
            };

            await using var context = new ManagementDataContext(options, Mock.Of<IMediator>(), tenantInfo);
            var userRepository = new UserRepository(context);
            var maybe = await userRepository.FindByEmailAddress(string.Empty);
            Assert.True(maybe.HasNoValue);
        }
    }
}