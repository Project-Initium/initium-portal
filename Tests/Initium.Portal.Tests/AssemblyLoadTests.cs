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
        [InlineData("Initium.Portal.Infrastructure")]
        [InlineData("Initium.Portal.Queries")]
        [InlineData("Initium.Portal.Web")]
        public void AssemblyLoad(string assemblyName)
        {
            var assemblyToTest = AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(assembly => assembly.GetName().Name == assemblyName);
            Assert.NotNull(assemblyToTest);
        }
    }
}