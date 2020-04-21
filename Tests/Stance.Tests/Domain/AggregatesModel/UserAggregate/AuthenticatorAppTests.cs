// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Stance.Domain.AggregatesModel.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.AggregatesModel.UserAggregate
{
    public class AuthenticatorAppTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var authenticatorApp = new AuthenticatorApp(TestVariables.AuthenticatorAppId, "key", TestVariables.Now);

            Assert.Equal(TestVariables.AuthenticatorAppId, authenticatorApp.Id);
            Assert.Equal("key", authenticatorApp.Key);
            Assert.Equal(TestVariables.Now, authenticatorApp.WhenEnrolled);
            Assert.Null(authenticatorApp.WhenRevoked);

            foreach (var prop in authenticatorApp.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(authenticatorApp, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void Constructor_GivenPrivateIsCalled_ExpectObjectCreated()
        {
            var authenticatorApp = (AuthenticatorApp)Activator.CreateInstance(typeof(AuthenticatorApp), true);
            Assert.NotNull(authenticatorApp);

            foreach (var prop in authenticatorApp.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(authenticatorApp, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void RevokeApp_GiveValidArguments_WhenRevokedIsSet()
        {
            var authenticatorApp = new AuthenticatorApp(TestVariables.AuthenticatorAppId, "key", TestVariables.Now);
            authenticatorApp.RevokeApp(TestVariables.Now.AddHours(1));

            Assert.Equal(TestVariables.Now.AddHours(1), authenticatorApp.WhenRevoked);
        }
    }
}