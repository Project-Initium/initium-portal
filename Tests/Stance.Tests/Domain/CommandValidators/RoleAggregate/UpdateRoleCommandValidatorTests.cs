// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Stance.Core.Contracts.Domain;
using Stance.Domain.Commands.RoleAggregate;
using Stance.Domain.CommandValidators.RoleAggregate;
using Xunit;

namespace Stance.Tests.Domain.CommandValidators.RoleAggregate
{
    public class UpdateRoleCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new UpdateRoleCommand(Guid.NewGuid(), new string('*', 5), new List<Guid>());
            var validator = new UpdateRoleCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenRoleIdIsEmpty_ExpectValidationFailure()
        {
            var cmd = new UpdateRoleCommand(Guid.Empty, new string('*', 5), new List<Guid>());
            var validator = new UpdateRoleCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "RoleId");
        }

        [Fact]
        public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
        {
            var cmd = new UpdateRoleCommand(Guid.NewGuid(), string.Empty, new List<Guid>());
            var validator = new UpdateRoleCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Name");
        }

        [Fact]
        public void Validate_GivenNameIsNull_ExpectValidationFailure()
        {
            var cmd = new UpdateRoleCommand(Guid.NewGuid(), null, new List<Guid>());
            var validator = new UpdateRoleCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Name");
        }


    }
}