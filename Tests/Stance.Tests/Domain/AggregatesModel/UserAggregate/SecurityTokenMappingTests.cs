// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Domain.AggregatesModel.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.AggregatesModel.UserAggregate
{
    public class SecurityTokenMappingTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var id = Guid.NewGuid();
            var created = DateTime.UtcNow;
            var expires = created.AddHours(1);
            var securityTokenMapping = new SecurityTokenMapping(
                id,
                SecurityTokenMapping.SecurityTokenPurpose.PasswordReset,
                created,
                expires);

            Assert.Equal(id, securityTokenMapping.Id);
            Assert.Equal(created, securityTokenMapping.WhenCreated);
            Assert.Equal(expires, securityTokenMapping.WhenExpires);
            Assert.Equal(SecurityTokenMapping.SecurityTokenPurpose.PasswordReset, securityTokenMapping.Purpose);
            Assert.Null(securityTokenMapping.WhenUsed);
            Assert.Equal(Convert.ToBase64String(id.ToByteArray()), securityTokenMapping.Token);
        }

        [Fact]
        public void MarkUsed_GiveValidArguments_PropertyIsSet()
        {
            var securityTokenMapping = new SecurityTokenMapping(
                Guid.NewGuid(),
                SecurityTokenMapping.SecurityTokenPurpose.PasswordReset,
                DateTime.UtcNow,
                DateTime.UtcNow);

            var used = DateTime.UtcNow;
            securityTokenMapping.MarkUsed(used);

            Assert.Equal(used, securityTokenMapping.WhenUsed);
        }
    }
}