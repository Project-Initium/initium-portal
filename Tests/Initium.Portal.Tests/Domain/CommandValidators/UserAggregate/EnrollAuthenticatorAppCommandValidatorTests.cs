// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Domain.CommandValidators.UserAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandValidators.UserAggregate
{
    public class EnrollAuthenticatorAppCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new EnrollAuthenticatorAppCommand("key", "code");
            var validator = new EnrollAuthenticatorAppCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenCodeIsEmpty_ExpectValidationFailure()
        {
            var cmd = new EnrollAuthenticatorAppCommand("key", string.Empty);
            var validator = new EnrollAuthenticatorAppCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Code");
        }

        [Fact]
        public void Validate_GivenCodeIsNull_ExpectValidationFailure()
        {
            var cmd = new EnrollAuthenticatorAppCommand("key", null);
            var validator = new EnrollAuthenticatorAppCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Code");
        }

        [Fact]
        public void Validate_GivenKeyIsEmpty_ExpectValidationFailure()
        {
            var cmd = new EnrollAuthenticatorAppCommand(string.Empty, "code");
            var validator = new EnrollAuthenticatorAppCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Key");
        }

        [Fact]
        public void Validate_GivenKeyIsNull_ExpectValidationFailure()
        {
            var cmd = new EnrollAuthenticatorAppCommand(null, "code");
            var validator = new EnrollAuthenticatorAppCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Key");
        }
    }
}