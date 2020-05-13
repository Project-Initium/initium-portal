// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.CommandValidators.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandValidators.UserAggregate
{
    public class VerifyAccountAndSetPasswordCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new VerifyAccountAndSetPasswordCommand("token", "new-password");
            var validator = new VerifyAccountAndSetPasswordCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenTokenIsNull_ExpectValidationFailure()
        {
            var cmd = new VerifyAccountAndSetPasswordCommand(null, "new-password");
            var validator = new VerifyAccountAndSetPasswordCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Token");
        }

        [Fact]
        public void Validate_GivenTokenIsEmpty_ExpectValidationFailure()
        {
            var cmd = new VerifyAccountAndSetPasswordCommand(string.Empty, "new-password");
            var validator = new VerifyAccountAndSetPasswordCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Token");
        }

        [Fact]
        public void Validate_GivenNewPasswordIsNull_ExpectValidationFailure()
        {
            var cmd = new VerifyAccountAndSetPasswordCommand("token", null);
            var validator = new VerifyAccountAndSetPasswordCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "NewPassword");
        }

        [Fact]
        public void Validate_GivenNewPasswordIsEmpty_ExpectValidationFailure()
        {
            var cmd = new VerifyAccountAndSetPasswordCommand("token", string.Empty);
            var validator = new VerifyAccountAndSetPasswordCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "NewPassword");
        }
    }
}