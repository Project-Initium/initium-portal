// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Domain.AggregatesModel.RoleAggregate;
using Xunit;

namespace Stance.Tests.Domain.AggregatesModel.RoleAggregate
{
    public class RoleResourceTests
    {
        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var id = Guid.NewGuid();
            var roleResource = new RoleResource(id);
            Assert.Equal(id, roleResource.Id);
        }
    }
}