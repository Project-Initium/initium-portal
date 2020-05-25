// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.SystemAlertAggregate;
using Initium.Portal.Domain.CommandValidators.SystemAlertAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandValidators.SystemAlertAggregate
{
    public class DeleteSystemAlertCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenSystemAlertIdIsEmpty_ExpectValidationFailure()
        {
            var cmd = new DeleteSystemAlertCommand(Guid.Empty);
            var validator = new DeleteSystemAlertCommandValidator();
            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.ErrorCode.Equals(ValidationCodes.FieldIsRequired) &&
                           failure.PropertyName == "SystemAlertId");
        }

        [Fact]
        public void Validate_GivenAllRequiredPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new DeleteSystemAlertCommand(TestVariables.SystemAlertId);
            var validator = new DeleteSystemAlertCommandValidator();
            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }
    }
}