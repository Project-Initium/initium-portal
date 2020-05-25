// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.SystemAlertAggregate;
using Initium.Portal.Domain.CommandValidators.SystemAlertAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandValidators.SystemAlertAggregate
{
    public class CreateNewSystemAlertCommandValidatorTests
    {
        [Fact]
        public void Validate_GiveNameIsEmpty_ExpectValidationFailure()
        {
            var cmd = new CreateNewSystemAlertCommand(
                string.Empty,
                "message",
                SystemAlertType.Critical,
                null,
                null);
            var validator = new CreateNewSystemAlertCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Name");
        }

        [Fact]
        public void Validate_GiveMessageIsEmpty_ExpectValidationFailure()
        {
            var cmd = new CreateNewSystemAlertCommand(
                "name",
                string.Empty,
                SystemAlertType.Critical,
                null,
                null);
            var validator = new CreateNewSystemAlertCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Message");
        }

        [Fact]
        public void Validate_GivenAllRequiredPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new CreateNewSystemAlertCommand(
                "name",
                "message",
                SystemAlertType.Critical,
                null,
                null);
            var validator = new CreateNewSystemAlertCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenNameIsNull_ExpectValidationFailure()
        {
            var cmd = new CreateNewSystemAlertCommand(
                null,
                "message",
                SystemAlertType.Critical,
                null,
                null);
            var validator = new CreateNewSystemAlertCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Name");
        }

        [Fact]
        public void Validate_GivenMessageIsNull_ExpectValidationFailure()
        {
            var cmd = new CreateNewSystemAlertCommand(
                "name",
                null,
                SystemAlertType.Critical,
                null,
                null);
            var validator = new CreateNewSystemAlertCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Message");
        }
    }
}