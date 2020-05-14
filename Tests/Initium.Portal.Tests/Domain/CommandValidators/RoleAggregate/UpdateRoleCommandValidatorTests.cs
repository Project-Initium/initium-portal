// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.RoleAggregate;
using Initium.Portal.Domain.CommandValidators.RoleAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandValidators.RoleAggregate
{
    public class UpdateRoleCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new UpdateRoleCommand(TestVariables.RoleId, "name", new List<Guid>());
            var validator = new UpdateRoleCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenRoleIdIsEmpty_ExpectValidationFailure()
        {
            var cmd = new UpdateRoleCommand(Guid.Empty, "name", new List<Guid>());
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
            var cmd = new UpdateRoleCommand(TestVariables.RoleId, string.Empty, new List<Guid>());
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
            var cmd = new UpdateRoleCommand(TestVariables.RoleId, null, new List<Guid>());
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