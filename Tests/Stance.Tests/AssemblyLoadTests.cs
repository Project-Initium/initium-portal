// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
using Xunit;

namespace Stance.Tests
{
    public class AssemblyLoadTests
    {
        [Theory]
        [InlineData("Stance.Core")]
        [InlineData("Stance.Domain")]
        [InlineData("Stance.Infrastructure")]
        [InlineData("Stance.Queries")]
        [InlineData("Stance.Web")]
        public void AssemblyLoad(string assemblyName)
        {
            var assemblyToTest = AppDomain.CurrentDomain.GetAssemblies()
                .SingleOrDefault(assembly => assembly.GetName().Name == assemblyName);
            Assert.NotNull(assemblyToTest);
        }
    }
}