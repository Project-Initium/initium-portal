// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Domain.AggregatesModel.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.AggregatesModel.UserAggregate
{
    public class AuthenticatorAppTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var id = Guid.NewGuid();
            var key = new string('*', 5);
            var whenEnrolled = DateTime.UtcNow;

            var authenticatorApp = new AuthenticatorApp(id, key, whenEnrolled);

            Assert.Equal(id, authenticatorApp.Id);
            Assert.Equal(key, authenticatorApp.Key);
            Assert.Equal(whenEnrolled, authenticatorApp.WhenEnrolled);
            Assert.Null(authenticatorApp.WhenRevoked);
        }

        [Fact]
        public void RevokeApp_GiveValidArguments_WhenRevokedIsSet()
        {
            var id = Guid.NewGuid();
            var key = new string('*', 5);
            var whenEnrolled = DateTime.UtcNow;
            var whenRevoked = DateTime.UtcNow.AddHours(1);

            var authenticatorApp = new AuthenticatorApp(id, key, whenEnrolled);
            authenticatorApp.RevokeApp(whenRevoked);

            Assert.Equal(whenRevoked, authenticatorApp.WhenRevoked);
        }
    }
}