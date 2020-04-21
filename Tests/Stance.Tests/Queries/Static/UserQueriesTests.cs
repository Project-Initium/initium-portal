// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MaybeMonad;
using Moq;
using Moq.Dapper;
using Stance.Core;
using Stance.Core.Contracts;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static;
using Stance.Queries.Static.TransferObjects;
using Stance.Queries.Static.TransferObjects.User;
using Xunit;

namespace Stance.Tests.Queries.Static
{
    public class UserQueriesTests
    {
        [Fact]
        public async Task CheckForPresenceOfAnyUser_GivenUserDoesExist_ExpectPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
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

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.CheckForPresenceOfAnyUser();
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfAnyUser_GivenUserDoesNotExist_ExpectNotPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<PresenceCheckDto<Guid>>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<PresenceCheckDto<Guid>>());

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.CheckForPresenceOfAnyUser();
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfAuthAppForCurrentUser_GivenNoUserIsAuthenticated_ExpectPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);

            var dbConnectionProvider = new Mock<IConnectionProvider>();

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.CheckForPresenceOfAuthAppForCurrentUser();
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfAuthAppForCurrentUser_GivenUserDoesExist_ExpectPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(
                Maybe.From(
                    new AuthenticatedUser(TestVariables.UserId, "email-address", "first-name", "last-name") as ISystemUser));
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

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.CheckForPresenceOfAuthAppForCurrentUser();
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfAuthAppForCurrentUser_GivenUserDoesNotExist_ExpectNotPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(
                Maybe.From(
                    new AuthenticatedUser(TestVariables.UserId, "email-address", "first-name", "last-name") as ISystemUser));
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<PresenceCheckDto<Guid>>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<PresenceCheckDto<Guid>>());

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.CheckForPresenceOfAuthAppForCurrentUser();
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfUserByEmailAddress_GivenUserDoesExist_ExpectPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
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

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.CheckForPresenceOfUserByEmailAddress("email-address");
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfUserByEmailAddress_GivenUserDoesNotExist_ExpectNotPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<PresenceCheckDto<Guid>>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<PresenceCheckDto<Guid>>());

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.CheckForPresenceOfUserByEmailAddress("email-address");
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task GetDetailsOfUserById_GivenMoreThenOneRecordIsFound_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<DetailedUserDto>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<DetailedUserDto>
                {
                    new DetailedUserDto(),
                    new DetailedUserDto(),
                });

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.GetDetailsOfUserById(Guid.NewGuid());
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetDetailsOfUserById_GivenNoDataIsFound_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<DetailedUserDto>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<DetailedUserDto>());

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.GetDetailsOfUserById(Guid.NewGuid());
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetDeviceInfoForCurrentUser_GivenDataIsFound_ExpectMaybeWithMappedData()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(
                Maybe.From(
                    new AuthenticatedUser(Guid.NewGuid(), string.Empty, string.Empty, string.Empty) as ISystemUser));
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<DeviceInfoDto>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<DeviceInfoDto>
                {
                    new DeviceInfoDto
                    {
                        Id = TestVariables.AuthenticatorDeviceId,
                        Name = "name",
                        WhenEnrolled = TestVariables.Now,
                        WhenLastUsed = TestVariables.Now.AddHours(1),
                    },
                });

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.GetDeviceInfoForCurrentUser();
            Assert.True(result.HasValue);
            Assert.Single(result.Value);
            Assert.Equal("name", result.Value.First().Name);
            Assert.Equal(TestVariables.AuthenticatorDeviceId, result.Value.First().Id);
            Assert.Equal(TestVariables.Now, result.Value.First().WhenEnrolled);
            Assert.Equal(TestVariables.Now.AddHours(1), result.Value.First().WhenLastUsed);
        }

        [Fact]
        public async Task GetDeviceInfoForCurrentUser_GivenNoDataIsFound_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(
                Maybe.From(
                    new AuthenticatedUser(Guid.NewGuid(), string.Empty, string.Empty, string.Empty) as ISystemUser));
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<DeviceInfoDto>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<DeviceInfoDto>());

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.GetDeviceInfoForCurrentUser();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetDeviceInfoForCurrentUser_GivenNoUserIsAuthenticated_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);

            var dbConnectionProvider = new Mock<IConnectionProvider>();

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.GetDeviceInfoForCurrentUser();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetProfileForCurrentUser_GivenMoreThenOneRecordIsFound_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<ProfileDto>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<ProfileDto>
                {
                    new ProfileDto(),
                    new ProfileDto(),
                });

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.GetProfileForCurrentUser();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetProfileForCurrentUser_GivenNoDataIsFound_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<ProfileDto>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<ProfileDto>());

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.GetProfileForCurrentUser();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetProfileForCurrentUser_GivenOneRecordIsFound_ExpectMaybeWithMappedData()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe.From(new Mock<ISystemUser>().Object));
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<ProfileDto>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<ProfileDto>
                {
                    new ProfileDto
                    {
                        FirstName = "first-name",
                        LastName = "last-name",
                    },
                });

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.GetProfileForCurrentUser();
            Assert.True(result.HasValue);
            Assert.Equal("first-name", result.Value.FirstName);
            Assert.Equal("last-name", result.Value.LastName);
        }

        [Fact]
        public async Task GetProfileForCurrentUser_GivenNoUserIsAuthenticated_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser).Returns(Maybe<ISystemUser>.Nothing);

            var dbConnectionProvider = new Mock<IConnectionProvider>();

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.GetProfileForCurrentUser();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetSystemProfileByUserId_GivenMoreThenOneRecordIsFound_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<SystemProfileDto>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<SystemProfileDto>
                {
                    new SystemProfileDto(),
                    new SystemProfileDto(),
                });

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.GetSystemProfileByUserId(Guid.NewGuid());
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetSystemProfileByUserId_GivenNoDataIsFound_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var connection = new Mock<IDbConnection>();
            connection.SetupDapperAsync(c => c.QueryAsync<SystemProfileDto>(
                    It.IsAny<string>(),
                    It.IsAny<DynamicParameters>(),
                    null, null, It.IsAny<CommandType>()))
                .ReturnsAsync(() => new List<SystemProfileDto>());

            var dbConnectionProvider = new Mock<IConnectionProvider>();
            dbConnectionProvider.Setup(x => x.GetConnection())
                .Returns(() => connection.Object);

            var userQueries = new UserQueries(currentAuthenticatedUserProvider.Object, dbConnectionProvider.Object);
            var result = await userQueries.GetSystemProfileByUserId(Guid.NewGuid());
            Assert.True(result.HasNoValue);
        }
    }
}