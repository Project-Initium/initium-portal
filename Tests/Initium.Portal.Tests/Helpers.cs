// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Initium.Portal.Core.Database;
using Initium.Portal.Core.MultiTenant;
using Initium.Portal.Infrastructure.EntityTypeConfigurationProviders;
using Initium.Portal.Infrastructure.Management.EntityTypeConfigurationProviders;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Initium.Portal.Tests
{
    public static class Helpers
    {
        internal static TEntity CreateEntity<TEntity>(object props)
        {
            var type = typeof(TEntity);
            var entity = Activator.CreateInstance(type, true);

            foreach (var prop in props.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (prop.PropertyType.Name == "List`1")
                {
                    type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                        .Single(x => x.Name.ToLower().Contains(prop.Name.ToLower()))
                        .SetValue(entity, prop.GetValue(props));
                }
                else
                {
                    type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Single(propertyInfo => string.Equals(propertyInfo.Name, prop.Name,
                            StringComparison.CurrentCultureIgnoreCase))
                        .SetValue(entity, prop.GetValue(props));
                }
            }

            return (TEntity)entity;
        }

        internal static Mock<IServiceProvider> AddCoreEntityTypeConfigurationProvider(this Mock<IServiceProvider> serviceProvider)
        {
            serviceProvider.Setup(x => x.GetService(typeof(IEnumerable<IEntityTypeConfigurationProvider>))).Returns(
                new List<IEntityTypeConfigurationProvider>
                {
                    new NotificationEntityTypeConfigurationProvider(TestVariables.TenantInfo),
                    new RoleEntityTypeConfigurationProvider(TestVariables.TenantInfo),
                    new SystemAlertEntityTypeConfigurationProvider(TestVariables.TenantInfo),
                    new UserEntityTypeConfigurationProvider(TestVariables.TenantInfo),
                }.ToArray);
            serviceProvider.Setup(x => x.GetService(typeof(FeatureBasedTenantInfo))).Returns(TestVariables.TenantInfo);

            return serviceProvider;
        }
        
        internal static Mock<IServiceProvider> AddManagementEntityTypeConfigurationProvider(this Mock<IServiceProvider> serviceProvider)
        {
            serviceProvider.Setup(x => x.GetService(typeof(IEnumerable<IEntityTypeConfigurationProvider>))).Returns(
                new List<IEntityTypeConfigurationProvider>
                {
                    new NotificationEntityTypeConfigurationProvider(TestVariables.TenantInfo),
                    new RoleEntityTypeConfigurationProvider(TestVariables.TenantInfo),
                    new SystemAlertEntityTypeConfigurationProvider(TestVariables.TenantInfo),
                    new UserEntityTypeConfigurationProvider(TestVariables.TenantInfo),
                    new TenantEntityTypeConfigurationProvider(),
                }.ToArray);
            serviceProvider.Setup(x => x.GetService(typeof(FeatureBasedTenantInfo))).Returns(TestVariables.TenantInfo);

            return serviceProvider;
        }
    }
}