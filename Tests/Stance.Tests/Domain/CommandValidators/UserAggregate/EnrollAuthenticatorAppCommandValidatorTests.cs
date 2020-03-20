// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.CommandValidators.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandValidators.UserAggregate
{
    public class EnrollAuthenticatorAppCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new EnrollAuthenticatorAppCommand(new string('*', 6), new string('*', 7));
            var validator = new EnrollAuthenticatorAppCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenCodeIsEmpty_ExpectValidationFailure()
        {
            var cmd = new EnrollAuthenticatorAppCommand(new string('*', 7), string.Empty);
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
            var cmd = new EnrollAuthenticatorAppCommand(new string('*', 7), null);
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
            var cmd = new EnrollAuthenticatorAppCommand(string.Empty, new string('*', 7));
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
            var cmd = new EnrollAuthenticatorAppCommand(null, new string('*', 7));
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