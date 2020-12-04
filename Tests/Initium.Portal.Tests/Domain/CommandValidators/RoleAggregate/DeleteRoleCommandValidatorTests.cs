// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.RoleAggregate;
using Initium.Portal.Domain.CommandValidators.RoleAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandValidators.RoleAggregate
{
    public class DeleteRoleCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new DeleteRoleCommand(TestVariables.RoleId);
            var validator = new DeleteRoleCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenRoleIdIsEmpty_ExpectValidationFailure()
        {
            var cmd = new DeleteRoleCommand(Guid.Empty);
            var validator = new DeleteRoleCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Single(result.Errors);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "RoleId");
        }
    }
}