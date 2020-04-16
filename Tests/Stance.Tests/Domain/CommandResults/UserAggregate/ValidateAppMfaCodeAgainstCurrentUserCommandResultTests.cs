﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Domain.CommandResults.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandResults.UserAggregate
{
    public class ValidateAppMfaCodeAgainstCurrentUserCommandResultTests
    {
        [Fact]
        public void CompletedConstructor_GiveValidArguments_PropertiesAreSet()
        {
            var result = new ValidateAppMfaCodeAgainstCurrentUserCommandResult(TestVariables.UserId);

            Assert.Equal(TestVariables.UserId, result.UserId);
            Assert.Equal(BaseAuthenticationProcessCommandResult.AuthenticationState.Completed, result.AuthenticationStatus);
        }

        [Fact]
        public void Constructor_GiveValidArguments_PropertiesAreSet()
        {
            var result = new ValidateAppMfaCodeAgainstCurrentUserCommandResult(TestVariables.UserId, BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaDeviceCode);

            Assert.Equal(TestVariables.UserId, result.UserId);
            Assert.Equal(BaseAuthenticationProcessCommandResult.AuthenticationState.AwaitingMfaDeviceCode, result.AuthenticationStatus);
        }
    }
}