// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Linq;
using System.Reflection;
using Initium.Portal.Domain.Commands.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain
{
    public class ConfigurationTests
    {
        [Fact]
        public void Configuration_GivenThereAreRequests_AllRequestsMustHaveValidation()
        {
            var commands = Assembly.GetAssembly(typeof(AuthenticateUserCommand)).GetTypes()
                .Where(x => x.GetInterfaces().Any(y => y.Name == "IRequest`1"));
            var validators = Assembly.GetAssembly(typeof(AuthenticateUserCommand)).GetTypes()
                .Where(x => x.BaseType != null && x.BaseType.Name == "AbstractValidator`1");

            Assert.All(commands, type => Assert.NotNull(validators.SingleOrDefault(x => x.BaseType.GenericTypeArguments.Count(y => y == type) == 1)));
        }
    }
}