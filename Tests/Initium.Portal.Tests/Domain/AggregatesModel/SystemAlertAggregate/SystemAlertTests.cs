// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Initium.Portal.Core.Constants;
using Initium.Portal.Domain.AggregatesModel.SystemAlertAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.AggregatesModel.SystemAlertAggregate
{
    public class SystemAlertTests
    {
        [Fact]
        public void Constructor_WhenEmpty_ExpectNavigationalPropertiesCreated()
        {
            var systemAlert = (SystemAlert)Activator.CreateInstance(typeof(SystemAlert), true);

            foreach (var prop in systemAlert.GetType().GetProperties()
                .Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(systemAlert, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void Constructor_WhenValid_ExpectPropertiesAreSet()
        {
            var systemAlert = new SystemAlert(
                TestVariables.SystemAlertId,
                "name",
                "message",
                SystemAlertType.Critical,
                whenToShow: TestVariables.Now.AddDays(-1),
                whenToHide: TestVariables.Now.AddDays(1));

            Assert.Equal(TestVariables.SystemAlertId, systemAlert.Id);
            Assert.Equal("message", systemAlert.Message);
            Assert.Equal("name", systemAlert.Name);
            Assert.Equal(SystemAlertType.Critical, systemAlert.Type);
            Assert.Equal(TestVariables.Now.AddDays(-1), systemAlert.WhenToShow);
            Assert.Equal(TestVariables.Now.AddDays(1), systemAlert.WhenToHide);

            foreach (var prop in systemAlert.GetType().GetProperties()
                .Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(systemAlert, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void UpdateDetails_WhenValid_ExpectPropertiesAreSet()
        {
            var systemAlert = new SystemAlert(
                TestVariables.SystemAlertId,
                "name",
                "message",
                SystemAlertType.Critical,
                whenToShow: TestVariables.Now.AddDays(-1),
                whenToHide: TestVariables.Now.AddDays(1));

            systemAlert.UpdateDetails("updated-name", "updated-message", SystemAlertType.High, TestVariables.Now.AddDays(-2),
                TestVariables.Now.AddDays(2));

            Assert.Equal("updated-name", systemAlert.Name);
            Assert.Equal("updated-message", systemAlert.Message);
            Assert.Equal(SystemAlertType.High, systemAlert.Type);
            Assert.Equal(TestVariables.Now.AddDays(-2), systemAlert.WhenToShow);
            Assert.Equal(TestVariables.Now.AddDays(2), systemAlert.WhenToHide);
        }
    }
}