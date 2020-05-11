// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Stance.Web.Controllers.Api.AuthApp.Models;
using Xunit;

namespace Stance.Tests.Web.Controllers.Api.AuthApp.Models
{
    public class EnrollAuthAppRequestValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var request = new EnrollAuthAppRequest
            {
                Code = "code",
                SharedKey = "shared-key",
            };
            var validator = new EnrollAuthAppRequest.EnrollAuthAppRequestValidator();
            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenCodeIsEmpty_ExpectValidationFailure()
        {
            var request = new EnrollAuthAppRequest
            {
                Code = string.Empty,
                SharedKey = "shared-key",
            };
            var validator = new EnrollAuthAppRequest.EnrollAuthAppRequestValidator();
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.PropertyName == "Code");
        }

        [Fact]
        public void Validate_GivenCodeIsNull_ExpectValidationFailure()
        {
            var request = new EnrollAuthAppRequest
            {
                Code = null,
                SharedKey = "shared-key",
            };
            var validator = new EnrollAuthAppRequest.EnrollAuthAppRequestValidator();
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.PropertyName == "Code");
        }

        [Fact]
        public void Validate_GivenSharedKeyIsEmpty_ExpectValidationFailure()
        {
            var request = new EnrollAuthAppRequest
            {
                Code = "code",
                SharedKey = string.Empty,
            };
            var validator = new EnrollAuthAppRequest.EnrollAuthAppRequestValidator();
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.PropertyName == "SharedKey");
        }

        [Fact]
        public void Validate_GivenSharedKeyIsNull_ExpectValidationFailure()
        {
            var request = new EnrollAuthAppRequest
            {
                Code = "code",
                SharedKey = null,
            };
            var validator = new EnrollAuthAppRequest.EnrollAuthAppRequestValidator();
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.PropertyName == "SharedKey");
        }
    }
}