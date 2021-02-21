// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Core.Settings;
using Initium.Portal.Queries;
using Initium.Portal.Queries.Entities;
using MaybeMonad;
using MediatR;
using Microsoft.Extensions.Options;
using Moq;
using Moq.EntityFrameworkCore;
using Xunit;

namespace Initium.Portal.Tests.Queries
{
    public class UserQueriesTests
    {
        [Fact]
        public async Task CheckForPresenceOfAnyUser_GivenUserDoesExist_ExpectPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>
                {
                    Helpers.CreateEntity<UserReadEntity>(new
                    {
                        Id = TestVariables.UserId,
                    }),
                });

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.CheckForPresenceOfAnyUser();
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfAnyUser_GivenUserDoesNotExist_ExpectNotPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>());

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.CheckForPresenceOfAnyUser();
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task GetProfileForCurrentUser_GivenDataIsFound_ExpectMaybeWithMappedData()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var systemUser = new Mock<ISystemUser>();
            systemUser.Setup(x => x.UserId).Returns(TestVariables.UserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>
                {
                    Helpers.CreateEntity<UserReadEntity>(new
                    {
                        Id = TestVariables.UserId,
                        FirstName = "first-name",
                        LastName = "last-name",
                    }),
                });

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.GetProfileForCurrentUser();
            Assert.True(result.HasValue);
            Assert.Equal("first-name", result.Value.FirstName);
            Assert.Equal("last-name", result.Value.LastName);
        }

        [Fact]
        public async Task GetProfileForCurrentUser_GivenDataIsNotFound_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var systemUser = new Mock<ISystemUser>();
            systemUser.Setup(x => x.UserId).Returns(TestVariables.UserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>());

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.GetProfileForCurrentUser();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetProfileForCurrentUser_GivenNoUserIsAuthenticated_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>());

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.GetProfileForCurrentUser();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task CheckForPresenceOfUserByEmailAddress_GivenUserDoesExist_ExpectPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>
                {
                    Helpers.CreateEntity<UserReadEntity>(new
                    {
                        EmailAddress = "email-address",
                    }),
                });
            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.CheckForPresenceOfUserByEmailAddress("email-address");
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfUserByEmailAddress_GivenUserDoesNotExist_ExpectNotPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>());
            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.CheckForPresenceOfUserByEmailAddress("email-address");
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task GetDetailsOfUserById_GivenNoDataIsFound_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>());

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.GetDetailsOfUserById(TestVariables.UserId);
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetDetailsOfUserById_GivenDataIsFound_ExpectMaybeWithMappedData()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>
                {
                    Helpers.CreateEntity<UserReadEntity>(new
                    {
                        Id = TestVariables.UserId,
                        EmailAddress = "email-address",
                        FirstName = "first-name",
                        LastName = "last-name",
                        IsLockable = true,
                        WhenCreated = TestVariables.Now.AddDays(-10),
                        WhenLastAuthenticated = TestVariables.Now.AddDays(-1),
                        WhenLocked = null as DateTime?,
                        IsAdmin = false,
                        WhenDisabled = TestVariables.Now,
                        Roles = new List<RoleReadEntity>
                        {
                            Helpers.CreateEntity<RoleReadEntity>(new
                            {
                                Id = TestVariables.RoleId,
                            }),
                        },
                    }),
                });

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.GetDetailsOfUserById(TestVariables.UserId);
            Assert.True(result.HasValue);
            Assert.Equal(TestVariables.UserId, result.Value.UserId);
            Assert.Equal("email-address", result.Value.EmailAddress);
            Assert.Equal("first-name", result.Value.FirstName);
            Assert.Equal("last-name", result.Value.LastName);
            Assert.True(result.Value.IsLockable);
            Assert.Equal(TestVariables.Now.AddDays(-10), result.Value.WhenCreated);
            Assert.Equal(TestVariables.Now.AddDays(-1), result.Value.WhenLastAuthenticated);
            Assert.Null(result.Value.WhenLocked);
            Assert.False(result.Value.IsAdmin);
            Assert.Equal(TestVariables.Now, result.Value.WhenDisabled);
            var resource = Assert.Single(result.Value.Resources);
            Assert.Equal(TestVariables.RoleId, resource);
        }

        [Fact]
        public async Task GetSystemProfileByUserId_GivenNoDataIsFound_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>());

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.GetSystemProfileByUserId(TestVariables.UserId);
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetSystemProfileByUserId_GivenDataIsFound_ExpectMaybeWithMappedData()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>
                {
                    Helpers.CreateEntity<UserReadEntity>(new
                    {
                        Id = TestVariables.UserId,
                        EmailAddress = "email-address",
                        FirstName = "first-name",
                        LastName = "last-name",
                        Roles = new List<RoleReadEntity>
                        {
                            Helpers.CreateEntity<RoleReadEntity>(new
                            {
                                Resources = new List<ResourceReadEntity>
                                {
                                    Helpers.CreateEntity<ResourceReadEntity>(new
                                    {
                                        NormalizedName = "normalized-name",
                                    }),
                                },
                            }),
                        },
                    }),
                });

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.GetSystemProfileByUserId(TestVariables.UserId);
            Assert.True(result.HasValue);
            Assert.Equal("email-address", result.Value.EmailAddress);
            Assert.Equal("first-name", result.Value.FirstName);
            Assert.Equal("last-name", result.Value.LastName);
            Assert.False(result.Value.IsAdmin);
            var resource = Assert.Single(result.Value.Resources);
            Assert.Equal("normalized-name", resource);
        }

        [Fact]
        public async Task
            CheckForPresenceOfAuthAppForCurrentUser_GivenNoUserIsAuthenticated_ExpectNotPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>
                {
                    Helpers.CreateEntity<UserReadEntity>(new
                    {
                        Id = TestVariables.UserId,
                    }),
                });

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.CheckForPresenceOfAuthAppForCurrentUser();
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfAuthAppForCurrentUser_GivenUserDoesExist_ExpectPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var systemUser = new Mock<ISystemUser>();
            systemUser.Setup(x => x.UserId).Returns(TestVariables.UserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>());

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.CheckForPresenceOfAuthAppForCurrentUser();
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfAuthAppForCurrentUser_GivenUserDoesNotHaveApp_ExpectNotPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var systemUser = new Mock<ISystemUser>();
            systemUser.Setup(x => x.UserId).Returns(TestVariables.UserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>
                {
                    Helpers.CreateEntity<UserReadEntity>(new
                    {
                        Id = TestVariables.UserId,
                    }),
                });

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.CheckForPresenceOfAuthAppForCurrentUser();
            Assert.False(result.IsPresent);
        }

        [Fact]
        public async Task CheckForPresenceOfAuthAppForCurrentUser_GivenUserDoesApp_ExpectPresentStatus()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var systemUser = new Mock<ISystemUser>();
            systemUser.Setup(x => x.UserId).Returns(TestVariables.UserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>
                {
                    Helpers.CreateEntity<UserReadEntity>(new
                    {
                        Id = TestVariables.UserId,
                        AuthenticatorApps = new List<AuthenticatorAppReadEntity>
                        {
                            Helpers.CreateEntity<AuthenticatorAppReadEntity>(new
                            {
                                Id = TestVariables.AuthenticatorAppId,
                            }),
                        },
                    }),
                });

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.CheckForPresenceOfAuthAppForCurrentUser();
            Assert.True(result.IsPresent);
        }

        [Fact]
        public async Task GetDeviceInfoForCurrentUser_GivenNoUserIsAuthenticated_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);
            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>());

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.GetDeviceInfoForCurrentUser();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetDeviceInfoForCurrentUser_GivenUserDoesExist_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);
            var systemUser = new Mock<ISystemUser>();
            systemUser.Setup(x => x.UserId).Returns(TestVariables.UserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>());

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.GetDeviceInfoForCurrentUser();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetDeviceInfoForCurrentUser_GivenUserHasNoDevices_ExpectMaybeWithNothing()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);
            var systemUser = new Mock<ISystemUser>();
            systemUser.Setup(x => x.UserId).Returns(TestVariables.UserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>
                {
                    Helpers.CreateEntity<UserReadEntity>(new
                    {
                        Id = TestVariables.UserId,
                    }),
                });

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.GetDeviceInfoForCurrentUser();
            Assert.True(result.HasNoValue);
        }

        [Fact]
        public async Task GetDeviceInfoForCurrentUser_GivenUserHasDevices_ExpectMaybeWithMappedData()
        {
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);
            var systemUser = new Mock<ISystemUser>();
            systemUser.Setup(x => x.UserId).Returns(TestVariables.UserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(systemUser.Object));

            var serviceProvider = new Mock<IServiceProvider>();
            serviceProvider.AddCoreReadEntityTypeConfigurationProvider();
            serviceProvider.Setup(x => x.GetService(typeof(IMediator)))
                .Returns(Mock.Of<IMediator>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantId = TestVariables.TenantId,
            });

            var context = new Mock<GenericDataContext>(serviceProvider.Object, Mock.Of<FeatureBasedTenantInfo>());
            context.Setup(x => x.Set<UserReadEntity>())
                .ReturnsDbSet(new List<UserReadEntity>
                {
                    Helpers.CreateEntity<UserReadEntity>(new
                    {
                        Id = TestVariables.UserId,
                        AuthenticatorDevices = new List<AuthenticatorDeviceReadEntity>
                        {
                            Helpers.CreateEntity<AuthenticatorDeviceReadEntity>(new
                            {
                                Id = TestVariables.AuthenticatorDeviceId,
                                Name = "name",
                                WhenEnrolled = TestVariables.Now.AddDays(-10),
                                WhenLastUsed = TestVariables.Now.AddDays(-3),
                            }),
                        },
                    }),
                });

            var userQueries = new UserQueryService(currentAuthenticatedUserProvider.Object, context.Object);
            var result = await userQueries.GetDeviceInfoForCurrentUser();
            Assert.True(result.HasValue);
            var device = Assert.Single(result.Value);
            Assert.NotNull(device);
            Assert.Equal(TestVariables.AuthenticatorDeviceId, device.Id);
            Assert.Equal("name", device.Name);
            Assert.Equal(TestVariables.Now.AddDays(-10), device.WhenEnrolled);
            Assert.Equal(TestVariables.Now.AddDays(-3), device.WhenLastUsed);
        }
    }
}