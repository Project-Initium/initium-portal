﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Initium.Portal.Common.Domain.AggregatesModel.TenantAggregate;
using Initium.Portal.Core.Constants;
using Xunit;

namespace Initium.Portal.Tests.Domain_Management.AggregatesModel.TenantAggregate
{
    public class TenantTests
    {
        [Fact]
        public void Constructor_WhenEmpty_ExpectNavigationalPropertiesCreated()
        {
            var tenant = (Tenant)Activator.CreateInstance(typeof(Tenant), true);

            foreach (var prop in tenant.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(tenant, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void Constructor_WhenValid_ExpectPropertiesAreSet()
        {
            var tenant = new Tenant(TestVariables.TenantId, "identifier", "name", "connection-string");

            Assert.Equal(TestVariables.TenantId, tenant.Id);
            Assert.Equal("identifier", tenant.Identifier);
            Assert.Equal("name", tenant.Name);
            Assert.Equal("connection-string", tenant.ConnectionString);

            foreach (var prop in tenant.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(tenant, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void UpdateDetails_WhenValid_ExpectPropertiesAreSet()
        {
            var tenant = new Tenant(TestVariables.TenantId, "identifier", "name", "connection-string");

            tenant.UpdateDetails("new-identifier", "new-name", "new-connection-string");

            Assert.Equal("new-identifier", tenant.Identifier);
            Assert.Equal("new-name", tenant.Name);
            Assert.Equal("new-connection-string", tenant.ConnectionString);
        }

        [Fact]
        public void SetTenantFeatures_GiveValidArguments_ExpectRolesToBeUpdated()
        {
            var tenant = new Tenant(TestVariables.TenantId, "identifier", "name", "connection-string");

            tenant.SetSystemFeatures(new List<SystemFeatures>
            {
                SystemFeatures.MfaApp,
                SystemFeatures.SystemAlerts,
            });

            tenant.SetSystemFeatures(new List<SystemFeatures>
            {
                SystemFeatures.MfaApp,
                SystemFeatures.MfaDevice,
                SystemFeatures.MfaDevice,
                SystemFeatures.UserNotifications,
            });

            Assert.Single(tenant.SystemFeatures, x => x == SystemFeatures.MfaApp);
            Assert.Single(tenant.SystemFeatures, x => x == SystemFeatures.MfaDevice);
            Assert.Single(tenant.SystemFeatures, x => x == SystemFeatures.UserNotifications);
            Assert.DoesNotContain(tenant.SystemFeatures, x => x == SystemFeatures.SystemAlerts);
        }

        [Fact]
        public void Disable_GiveValidArguments_ExpectTenantToBeDisabled()
        {
            var tenant = new Tenant(TestVariables.TenantId, "identifier", "name", "connection-string");
            tenant.Disable(TestVariables.Now);

            Assert.Equal(TestVariables.Now, tenant.WhenDisabled);
        }

        [Fact]
        public void Enable_GiveValidArguments_ExpectTenantToBeEnabled()
        {
            var tenant = new Tenant(TestVariables.TenantId, "identifier", "name", "connection-string");
            tenant.Disable(TestVariables.Now);
            tenant.Enable();

            Assert.Null(tenant.WhenDisabled);
        }
    }
}