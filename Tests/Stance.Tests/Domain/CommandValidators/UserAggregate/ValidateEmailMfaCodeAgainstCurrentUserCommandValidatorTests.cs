// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.CommandValidators.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandValidators.UserAggregate
{
    public class ValidateEmailMfaCodeAgainstCurrentUserCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new ValidateEmailMfaCodeAgainstCurrentUserCommand(new string('*', 6));
            var validator = new ValidateEmailMfaCodeAgainstCurrentUserCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenCodeIsEmpty_ExpectValidationFailure()
        {
            var cmd = new ValidateEmailMfaCodeAgainstCurrentUserCommand(string.Empty);
            var validator = new ValidateEmailMfaCodeAgainstCurrentUserCommandValidator();
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
            var cmd = new ValidateEmailMfaCodeAgainstCurrentUserCommand(null);
            var validator = new ValidateEmailMfaCodeAgainstCurrentUserCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Code");
        }
    }
}