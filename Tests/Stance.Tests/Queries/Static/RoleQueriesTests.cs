// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Moq;
using Moq.Dapper;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static;
using Stance.Queries.Static.TransferObjects;
using Stance.Queries.Static.TransferObjects.Role;
using Xunit;

namespace Stance.Tests.Queries.Static
{
    public class RoleQueriesTests
    {
        [Fact]
        public async Task CheckForPresenceOfRoleByName_GivenRoleDoesNotExist_ExpectNotPresentStatus()
        {
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<PresenceCheckDto<string>>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<PresenceCheckDto<string>>());

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var roleQueries = new RoleQueries(dbConnectionProvider.Object);
            var result = await roleQueries.CheckForPresenceOfRoleByName("name");
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfRoleByName_GivenRoleDoesExist_ExpectPresentStatus()
        {
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<PresenceCheckDto<Guid>>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<PresenceCheckDto<Guid>>
                {
                    new PresenceCheckDto<Guid>(),
                });

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var roleQueries = new RoleQueries(dbConnectionProvider.Object);
            var result = await roleQueries.CheckForPresenceOfRoleByName("name");
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task CheckForRoleUsageById_GivenRoleIsInUse_ExpectPresentStatus()
        {
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<PresenceCheckDto<Guid>>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<PresenceCheckDto<Guid>>
                {
                    new PresenceCheckDto<Guid>(),
                });

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var roleQueries = new RoleQueries(dbConnectionProvider.Object);
            var result = await roleQueries.CheckForRoleUsageById(Guid.NewGuid());
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task CheckForRoleUsageById_GivenRoleIsNotInUse_ExpectNotPresentStatus()
        {
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<PresenceCheckDto<Guid>>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<PresenceCheckDto<Guid>>());

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var roleQueries = new RoleQueries(dbConnectionProvider.Object);
            var result = await roleQueries.CheckForRoleUsageById(Guid.NewGuid());
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task GetDetailsOfRoleById_GivenNoDataIsFound_ExpectMaybeWithNothing()
        {
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<DetailedRoleDto>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<DetailedRoleDto>());

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var roleQueries = new RoleQueries(dbConnectionProvider.Object);
            var result = await roleQueries.GetDetailsOfRoleById(Guid.NewGuid());
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetDetailsOfRoleById_GivenMoreThenOneRecordIsFound_ExpectMaybeWithNothing()
        {
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<DetailedRoleDto>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<DetailedRoleDto>
                {
                    new DetailedRoleDto(),
                    new DetailedRoleDto(),
                });

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var roleQueries = new RoleQueries(dbConnectionProvider.Object);
            var result = await roleQueries.GetDetailsOfRoleById(Guid.NewGuid());
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetNestedSimpleResources_GivenNoDataIsFound_ExpectMaybeWithNothing()
        {
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<SimpleResourceDto>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<SimpleResourceDto>());

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var roleQueries = new RoleQueries(dbConnectionProvider.Object);
            var result = await roleQueries.GetNestedSimpleResources();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetNestedSimpleResources_GivenDataIsFound_ExpectMaybeWithMappedData()
        {
            var parentDto1 = new SimpleResourceDto
            {
                Id = Guid.NewGuid(),
                Name = "parent-dto-1",
            };
            var parentDto2 = new SimpleResourceDto
            {
                Id = Guid.NewGuid(),
                Name = "parent-dto-2",
            };
            var childDto1 = new SimpleResourceDto
            {
                Id = Guid.NewGuid(),
                Name = "child-dto-1",
                ParentResourceId = parentDto1.Id,
            };
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<SimpleResourceDto>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<SimpleResourceDto>
                {
                    parentDto1,
                    parentDto2,
                    childDto1,
                });

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var roleQueries = new RoleQueries(dbConnectionProvider.Object);
            var result = await roleQueries.GetNestedSimpleResources();
            Assert.True(result.HasValue);
            Assert.Equal(2, result.Value.Count);
            var parent1 = result.Value.FirstOrDefault(x => x.Name == "parent-dto-1");
            Assert.NotNull(parent1);
            Assert.Equal(parentDto1.Name, parent1.Name);
            Assert.Equal(parentDto1.Id, parent1.Id);
            Assert.Single(parent1.SimpleResources);
            Assert.Equal(childDto1.Name, parent1.SimpleResources.First().Name);
            Assert.Equal(childDto1.Id, parent1.SimpleResources.First().Id);
        }

        [Fact]
        public async Task GetSimpleRoles_GivenNoDataIsFound_ExpectMaybeWithNothing()
        {
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<SimpleRoleDto>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<SimpleRoleDto>());

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var roleQueries = new RoleQueries(dbConnectionProvider.Object);
            var result = await roleQueries.GetSimpleRoles();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetSimpleRoles_GivenDataIsFound_ExpectMaybeWithMappedData()
        {
            var dto = new SimpleRoleDto
            {
                Id = Guid.NewGuid(),
                Name = "Name",
            };

            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<SimpleRoleDto>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<SimpleRoleDto>
                {
                    dto,
                });

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var roleQueries = new RoleQueries(dbConnectionProvider.Object);
            var result = await roleQueries.GetSimpleRoles();
            Assert.True(result.HasValue);
            Assert.Single(result.Value);
            Assert.Equal(dto.Name, result.Value.First().Name);
            Assert.Equal(dto.Id, result.Value.First().Id);
        }
    }
}