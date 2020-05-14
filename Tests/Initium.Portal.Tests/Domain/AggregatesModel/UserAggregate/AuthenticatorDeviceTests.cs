// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.AggregatesModel.UserAggregate
{
    public class AuthenticatorDeviceTests
    {
        [Fact]
        public void Constructor_GivenPrivateIsCalled_ExpectObjectCreated()
        {
            var authenticatorApp = (AuthenticatorDevice)Activator.CreateInstance(typeof(AuthenticatorDevice), true);
            Assert.NotNull(authenticatorApp);

            foreach (var prop in authenticatorApp.GetType().GetProperties()
                .Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(authenticatorApp, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var authenticatorDevice = new AuthenticatorDevice(
                TestVariables.AuthenticatorDeviceId, TestVariables.Now, TestVariables.AuthenticatorDevicePublicKey,
                TestVariables.AuthenticatorDeviceCredentialId, TestVariables.AuthenticatorDeviceAaguid, 1,
                "name", "cred-type");

            Assert.Equal(TestVariables.AuthenticatorDeviceId, authenticatorDevice.Id);
            Assert.Equal(TestVariables.Now, authenticatorDevice.WhenEnrolled);
            Assert.Equal(TestVariables.AuthenticatorDevicePublicKey, authenticatorDevice.PublicKey);
            Assert.Equal(TestVariables.AuthenticatorDeviceCredentialId, authenticatorDevice.CredentialId);
            Assert.Equal(TestVariables.AuthenticatorDeviceAaguid, authenticatorDevice.Aaguid);
            Assert.Equal(1, authenticatorDevice.Counter);
            Assert.Equal("name", authenticatorDevice.Name);
            Assert.Equal("cred-type", authenticatorDevice.CredType);
            Assert.Null(authenticatorDevice.WhenRevoked);

            foreach (var prop in authenticatorDevice.GetType().GetProperties()
                .Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(authenticatorDevice, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void RevokeDevice_GiveValidArguments_DeviceIsMarkedAsRevoked()
        {
            var authenticatorDevice = new AuthenticatorDevice(
                TestVariables.AuthenticatorDeviceId, TestVariables.Now, TestVariables.AuthenticatorDevicePublicKey,
                TestVariables.AuthenticatorDeviceCredentialId, TestVariables.AuthenticatorDeviceAaguid, 1,
                "name", "cred-type");

            authenticatorDevice.RevokeDevice(TestVariables.Now.AddHours(1));
            Assert.Equal(TestVariables.Now.AddHours(1), authenticatorDevice.WhenRevoked);
        }

        [Fact]
        public void UpdateCounter_GiveValidArguments_PropertiesAreUpdated()
        {
            var authenticatorDevice = new AuthenticatorDevice(
                TestVariables.AuthenticatorDeviceId, TestVariables.Now, TestVariables.AuthenticatorDevicePublicKey,
                TestVariables.AuthenticatorDeviceCredentialId, TestVariables.AuthenticatorDeviceAaguid, 1,
                "name", "cred-type");

            authenticatorDevice.UpdateCounter(3, TestVariables.Now.AddHours(1));
            Assert.Equal(TestVariables.Now.AddHours(1), authenticatorDevice.WhenLastUsed);
            Assert.Equal(3, authenticatorDevice.Counter);
        }
    }
}