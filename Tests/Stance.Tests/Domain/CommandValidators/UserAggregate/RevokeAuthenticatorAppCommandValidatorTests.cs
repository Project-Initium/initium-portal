// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.CommandValidators.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandValidators.UserAggregate
{
    public class RevokeAuthenticatorAppCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new RevokeAuthenticatorAppCommand("password");
            var validator = new RevokeAuthenticatorAppCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenPasswordIsEmpty_ExpectValidationFailure()
        {
            var cmd = new RevokeAuthenticatorAppCommand(string.Empty);
            var validator = new RevokeAuthenticatorAppCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Password");
        }

        [Fact]
        public void Validate_GivenPasswordIsNull_ExpectValidationFailure()
        {
            var cmd = new RevokeAuthenticatorAppCommand(null);
            var validator = new RevokeAuthenticatorAppCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Password");
        }
    }
}