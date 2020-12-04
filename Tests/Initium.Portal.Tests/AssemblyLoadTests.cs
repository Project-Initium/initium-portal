// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Xunit;

namespace Initium.Portal.Tests
{
    public class AssemblyLoadTests
    {
        [Theory]
        [InlineData("Initium.Portal.Core")]
        [InlineData("Initium.Portal.Domain")]
        [InlineData("Initium.Portal.Domain.Management")]
        [InlineData("Initium.Portal.Domain.Tenant")]
        [InlineData("Initium.Portal.Infrastructure")]
        [InlineData("Initium.Portal.Infrastructure.Management")]
        [InlineData("Initium.Portal.Infrastructure.Tenant")]
        [InlineData("Initium.Portal.Queries")]
        [InlineData("Initium.Portal.Queries.Management")]
        [InlineData("Initium.Portal.Queries.Tenant")]
        [InlineData("Initium.Portal.Web")]
        [InlineData("Initium.Portal.Web.Management")]
        [InlineData("Initium.Portal.Web.Tenant")]
        public void AssemblyLoad(string assemblyName)
        {
            var assemblyToTest = AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(assembly => assembly.GetName().Name == assemblyName);
            Assert.NotNull(assemblyToTest);
        }
    }
}