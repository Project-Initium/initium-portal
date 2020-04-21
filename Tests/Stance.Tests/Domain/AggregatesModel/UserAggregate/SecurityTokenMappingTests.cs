// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Stance.Domain.AggregatesModel.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.AggregatesModel.UserAggregate
{
    public class SecurityTokenMappingTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var securityTokenMapping = new SecurityTokenMapping(
                TestVariables.SecurityTokenMappingId,
                SecurityTokenMapping.SecurityTokenPurpose.PasswordReset,
                TestVariables.Now,
                TestVariables.Now.AddHours(1));

            Assert.Equal(TestVariables.SecurityTokenMappingId, securityTokenMapping.Id);
            Assert.Equal(TestVariables.Now, securityTokenMapping.WhenCreated);
            Assert.Equal(TestVariables.Now.AddHours(1), securityTokenMapping.WhenExpires);
            Assert.Equal(SecurityTokenMapping.SecurityTokenPurpose.PasswordReset, securityTokenMapping.Purpose);
            Assert.Null(securityTokenMapping.WhenUsed);
            Assert.Equal(Convert.ToBase64String(TestVariables.SecurityTokenMappingId.ToByteArray()), securityTokenMapping.Token);

            foreach (var prop in securityTokenMapping.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(securityTokenMapping, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void Constructor_GivenPrivateIsCalled_ExpectObjectCreated()
        {
            var securityTokenMapping = (SecurityTokenMapping)Activator.CreateInstance(typeof(SecurityTokenMapping), true);
            Assert.NotNull(securityTokenMapping);

            foreach (var prop in securityTokenMapping.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(securityTokenMapping, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void MarkUsed_GiveValidArguments_PropertyIsSet()
        {
            var securityTokenMapping = new SecurityTokenMapping(
                TestVariables.SecurityTokenMappingId,
                SecurityTokenMapping.SecurityTokenPurpose.PasswordReset,
                TestVariables.Now,
                TestVariables.Now.AddHours(1));

            securityTokenMapping.MarkUsed(TestVariables.Now.AddMinutes(30));

            Assert.Equal(TestVariables.Now.AddMinutes(30), securityTokenMapping.WhenUsed);
        }
    }
}