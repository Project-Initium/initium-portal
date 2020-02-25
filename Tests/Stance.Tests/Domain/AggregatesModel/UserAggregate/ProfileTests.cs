// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Domain.AggregatesModel.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.AggregatesModel.UserAggregate
{
    public class ProfileTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var id = Guid.NewGuid();
            var firstName = new string('*', 5);
            var lastName = new string('*', 6);

            var profile = new Profile(id, firstName, lastName);

            Assert.Equal(id, profile.Id);
            Assert.Equal(firstName, profile.FirstName);
            Assert.Equal(lastName, profile.LastName);
        }

        [Fact]
        public void UpdateProfile_GiveValidArguments_PropertiesAreUpdated()
        {
            var id = Guid.NewGuid();
            var firstName = new string('*', 5);
            var lastName = new string('*', 6);

            var profile = new Profile(id, string.Empty, string.Empty);

            profile.UpdateProfile(firstName, lastName);

            Assert.Equal(firstName, profile.FirstName);
            Assert.Equal(lastName, profile.LastName);
        }
    }
}