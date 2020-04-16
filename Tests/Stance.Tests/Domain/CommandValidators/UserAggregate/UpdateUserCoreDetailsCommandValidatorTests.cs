// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.UserAggregate;
using Stance.Domain.CommandValidators.UserAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandValidators.UserAggregate
{
    public class UpdateUserCoreDetailsCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new UpdateUserCoreDetailsCommand(TestVariables.UserId, "a@b.com", "first-name", "last-name", true, true, new List<Guid>());
            var validator = new UpdateUserCoreDetailsCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenEmailAddressIsEmpty_ExpectValidationFailure()
        {
            var cmd = new UpdateUserCoreDetailsCommand(TestVariables.UserId, string.Empty, "first-name", "last-name", true, true, new List<Guid>());
            var validator = new UpdateUserCoreDetailsCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenEmailAddressIsNull_ExpectValidationFailure()
        {
            var cmd = new UpdateUserCoreDetailsCommand(TestVariables.UserId, null, "first-name", "last-name", true, true, new List<Guid>());
            var validator = new UpdateUserCoreDetailsCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenEmailAddressIsNotValidEmailAddress_ExpectValidationFailure()
        {
            var cmd = new UpdateUserCoreDetailsCommand(TestVariables.UserId, "email-address",  "first-name", "last-name", true, true, new List<Guid>());
            var validator = new UpdateUserCoreDetailsCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.ValueMustBeAnEmailAddress) &&
                           failure.PropertyName == "EmailAddress");
        }

        [Fact]
        public void Validate_GivenUserIdIsEmpty_ExpectValidationFailure()
        {
            var cmd = new UpdateUserCoreDetailsCommand(Guid.Empty, "a@b.com", "first-name", "last-name", true, true, new List<Guid>());
            var validator = new UpdateUserCoreDetailsCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "UserId");
        }

        [Fact]
        public void Validate_GivenFirstNameIsEmpty_ExpectValidationFailure()
        {
            var cmd = new UpdateUserCoreDetailsCommand(TestVariables.UserId, "a@b.com", string.Empty, "last-name", true, true, new List<Guid>());
            var validator = new UpdateUserCoreDetailsCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "FirstName");
        }

        [Fact]
        public void Validate_GivenFirstNameIsNull_ExpectValidationFailure()
        {
            var cmd = new UpdateUserCoreDetailsCommand(TestVariables.UserId, "a@b.com", null, "last-name", true, true, new List<Guid>());
            var validator = new UpdateUserCoreDetailsCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "FirstName");
        }

        [Fact]
        public void Validate_GivenLastNameIsEmpty_ExpectValidationFailure()
        {
            var cmd = new UpdateUserCoreDetailsCommand(TestVariables.UserId, "a@b.com", "first-name", string.Empty, true, true, new List<Guid>());
            var validator = new UpdateUserCoreDetailsCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "LastName");
        }

        [Fact]
        public void Validate_GivenLastNameIsNull_ExpectValidationFailure()
        {
            var cmd = new UpdateUserCoreDetailsCommand(TestVariables.UserId, "a@b.com", "first-name", null, true, true, new List<Guid>());
            var validator = new UpdateUserCoreDetailsCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "LastName");
        }
    }
}