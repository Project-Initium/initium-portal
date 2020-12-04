// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Initium.Portal.Common.Domain.CommandValidators.TenantAggregate;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Contracts.Domain;
using Xunit;

namespace Initium.Portal.Tests.Domain_Management.CommandValidators.TenantAggregate
{
    public class UpdateTenantCommandValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var cmd = new UpdateTenantCommand(
                TestVariables.TenantId,
                "identifier",
                "name",
                new List<SystemFeatures>());
            var validator = new UpdateTenantCommandValidator();

            var result = validator.Validate(cmd);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenTenantIdIsEmpty_ExpectFail()
        {
            var cmd = new UpdateTenantCommand(
                Guid.Empty,
                "identifier",
                "name",
                new List<SystemFeatures>());
            var validator = new UpdateTenantCommandValidator();

            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorCode == ValidationCodes.FieldIsRequired && x.PropertyName == "TenantId");
        }

        [Fact]
        public void Validate_GivenIdentifierIsNull_ExpectFail()
        {
            var cmd = new UpdateTenantCommand(
                TestVariables.TenantId,
                null,
                "name",
                new List<SystemFeatures>());
            var validator = new UpdateTenantCommandValidator();

            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorCode == ValidationCodes.FieldIsRequired && x.PropertyName == "Identifier");
        }

        [Fact]
        public void Validate_GivenIdentifierIsEmpty_ExpectFail()
        {
            var cmd = new UpdateTenantCommand(
                TestVariables.TenantId,
                string.Empty,
                "name",
                new List<SystemFeatures>());
            var validator = new UpdateTenantCommandValidator();

            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorCode == ValidationCodes.FieldIsRequired && x.PropertyName == "Identifier");
        }

        [Fact]
        public void Validate_GivenNameIsNull_ExpectFail()
        {
            var cmd = new UpdateTenantCommand(
                TestVariables.TenantId,
                "identifier",
                null,
                new List<SystemFeatures>());
            var validator = new UpdateTenantCommandValidator();

            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorCode == ValidationCodes.FieldIsRequired && x.PropertyName == "Name");
        }

        [Fact]
        public void Validate_GivenNameIsEmpty_ExpectFail()
        {
            var cmd = new UpdateTenantCommand(
                TestVariables.TenantId,
                "identifier",
                string.Empty,
                new List<SystemFeatures>());
            var validator = new UpdateTenantCommandValidator();

            var result = validator.Validate(cmd);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.ErrorCode == ValidationCodes.FieldIsRequired && x.PropertyName == "Name");
        }
    }
}