// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using Initium.Portal.Core.Contracts.Domain;
using Initium.Portal.Domain.Commands.TenantAggregate;
using Initium.Portal.Domain.CommandValidators.TenantAggregate;
using Xunit;

namespace Initium.Portal.Tests.Domain.CommandValidators.TenantAggregate
{
    public class CreateTenantCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new CreateTenantCommand(TestVariables.TenantId, "identifier", "name", "connection-string");
            var validator = new CreateTenantCommandValidator();

            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenTenantIdIsEmpty_ExpectFail()
        {
            var cmd = new CreateTenantCommand(Guid.Empty, "identifier", "name", "connection-string");
            var validator = new CreateTenantCommandValidator();

            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorCode == ValidationCodes.FieldIsRequired && x.PropertyName == "TenantId");
        }

        [Fact]
        public void Validate_GivenIdentifierIsNull_ExpectFail()
        {
            var cmd = new CreateTenantCommand(TestVariables.TenantId, null, "name", "connection-string");
            var validator = new CreateTenantCommandValidator();

            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorCode == ValidationCodes.FieldIsRequired && x.PropertyName == "Identifier");
        }

        [Fact]
        public void Validate_GivenIdentifierIsEmpty_ExpectFail()
        {
            var cmd = new CreateTenantCommand(TestVariables.TenantId, string.Empty, "name", "connection-string");
            var validator = new CreateTenantCommandValidator();

            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorCode == ValidationCodes.FieldIsRequired && x.PropertyName == "Identifier");
        }

        [Fact]
        public void Validate_GivenNameIsNull_ExpectFail()
        {
            var cmd = new CreateTenantCommand(TestVariables.TenantId, "identifier", null, "connection-string");
            var validator = new CreateTenantCommandValidator();

            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorCode == ValidationCodes.FieldIsRequired && x.PropertyName == "Name");
        }

        [Fact]
        public void Validate_GivenNameIsEmpty_ExpectFail()
        {
            var cmd = new CreateTenantCommand(TestVariables.TenantId, "identifier", string.Empty, "connection-string");
            var validator = new CreateTenantCommandValidator();

            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorCode == ValidationCodes.FieldIsRequired && x.PropertyName == "Name");
        }

        [Fact]
        public void Validate_GivenConnectionStringIsNull_ExpectFail()
        {
            var cmd = new CreateTenantCommand(TestVariables.TenantId, "identifier", "name", null);
            var validator = new CreateTenantCommandValidator();

            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorCode == ValidationCodes.FieldIsRequired && x.PropertyName == "ConnectionString");
        }

        [Fact]
        public void Validate_GivenConnectionStringIsEmpty_ExpectFail()
        {
            var cmd = new CreateTenantCommand(TestVariables.TenantId, "identifier", "name", string.Empty);
            var validator = new CreateTenantCommandValidator();

            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorCode == ValidationCodes.FieldIsRequired && x.PropertyName == "ConnectionString");
        }
    }
}