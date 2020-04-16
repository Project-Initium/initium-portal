﻿// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.CommandValidators.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandValidators.UserAggregate
{
    public class RevokeAuthenticatorDeviceCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new RevokeAuthenticatorDeviceCommand(TestVariables.AuthenticatorDeviceId);
            var validator = new RevokeAuthenticatorDeviceCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenDeviceIdIsEmpty_ExpectValidationFailure()
        {
            var cmd = new RevokeAuthenticatorDeviceCommand(Guid.Empty);
            var validator = new RevokeAuthenticatorDeviceCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "DeviceId");
        }
    }
}