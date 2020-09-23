// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.TenantAggregate;
using Initium.Portal.Domain.CommandValidators.TenantAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandValidators.TenantAggregate
{
    public class DisableTenantCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new DisableTenantCommand(TestVariables.TenantId);
            var validator = new DisableTenantCommandValidator();

            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenTenantIdIsEmpty_ExpectFail()
        {
            var cmd = new DisableTenantCommand(Guid.Empty);
            var validator = new DisableTenantCommandValidator();

            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorCode == ValidationCodes.FieldIsRequired && x.PropertyName == "TenantId");
        }
    }
}