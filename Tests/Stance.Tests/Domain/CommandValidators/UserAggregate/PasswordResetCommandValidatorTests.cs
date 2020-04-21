// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.CommandValidators.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandValidators.UserAggregate
{
    public class PasswordResetCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new PasswordResetCommand("token", "new-password");
            var validator = new PasswordResetCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenNewPasswordIsEmpty_ExpectValidationFailure()
        {
            var cmd = new PasswordResetCommand("token", string.Empty);
            var validator = new PasswordResetCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "NewPassword");
        }

        [Fact]
        public void Validate_GivenNewPasswordIsNull_ExpectValidationFailure()
        {
            var cmd = new PasswordResetCommand("token", null);
            var validator = new PasswordResetCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "NewPassword");
        }

        [Fact]
        public void Validate_GivenTokenIsEmpty_ExpectValidationFailure()
        {
            var cmd = new PasswordResetCommand(string.Empty, "new-password");
            var validator = new PasswordResetCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Token");
        }

        [Fact]
        public void Validate_GivenTokenIsNull_ExpectValidationFailure()
        {
            var cmd = new PasswordResetCommand(null, "new-password");
            var validator = new PasswordResetCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Token");
        }
    }
}