// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Initium.Portal.Domain.AggregatesModel.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.AggregatesModel.UserAggregate
{
    public class ProfileTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var profile = new Profile(TestVariables.UserId, "first-name", "last-name");

            Assert.Equal(TestVariables.UserId, profile.Id);
            Assert.Equal("first-name", profile.FirstName);
            Assert.Equal("last-name", profile.LastName);

            foreach (var prop in profile.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(profile, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void Constructor_GivenPrivateIsCalled_ExpectObjectCreated()
        {
            var profile = (Profile)Activator.CreateInstance(typeof(Profile), true);
            Assert.NotNull(profile);

            foreach (var prop in profile.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(profile, null);
                Assert.False(val == null, $"{prop.Name} is null");
            }
        }

        [Fact]
        public void UpdateProfile_GiveValidArguments_PropertiesAreUpdated()
        {
            var profile = new Profile(TestVariables.UserId, string.Empty, string.Empty);

            profile.UpdateProfile("first-name", "last-name");

            Assert.Equal("first-name", profile.FirstName);
            Assert.Equal("last-name", profile.LastName);
        }
    }
}