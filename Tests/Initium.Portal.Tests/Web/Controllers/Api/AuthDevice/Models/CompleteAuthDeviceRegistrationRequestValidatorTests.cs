// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Web.Controllers.Api.AuthDevice.Models;
using Xunit;

namespace Initium.Portal.Tests.Web.Controllers.Api.AuthDevice.Models
{
    public class CompleteAuthDeviceRegistrationRequestValidatorTests
    {
        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var request = new CompleteAuthDeviceRegistrationRequest
            {
                Name = "name",
            };
            var validator = new CompleteAuthDeviceRegistrationRequest.CompleteAuthDeviceRegistrationRequestValidator();
            var result = validator.Validate(request);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
        {
            var request = new CompleteAuthDeviceRegistrationRequest
            {
                Name = string.Empty,
            };
            var validator = new CompleteAuthDeviceRegistrationRequest.CompleteAuthDeviceRegistrationRequestValidator();
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.PropertyName == "Name");
        }

        [Fact]
        public void Validate_GivenPasswordIsNull_ExpectValidationFailure()
        {
            var request = new CompleteAuthDeviceRegistrationRequest
            {
                Name = null,
            };
            var validator = new CompleteAuthDeviceRegistrationRequest.CompleteAuthDeviceRegistrationRequestValidator();
            var result = validator.Validate(request);
            Assert.False(result.IsValid);
            Assert.Contains(
                result.Errors,
                failure => failure.PropertyName == "Name");
        }
    }
}