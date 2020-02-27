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
    public class CreateRoleCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new CreateRoleCommand(new string('*', 5), new List<Guid>());
            var validator = new CreateRoleCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
        {
            var cmd = new CreateRoleCommand(string.Empty, new List<Guid>());
            var validator = new CreateRoleCommandValidator();
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
            var cmd = new CreateRoleCommand(null, new List<Guid>());
            var validator = new CreateRoleCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "Name");
        }
    }
}