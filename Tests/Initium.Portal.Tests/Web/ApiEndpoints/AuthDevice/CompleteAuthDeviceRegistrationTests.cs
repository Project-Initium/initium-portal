// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using Initium.Portal.Web.ApiEndpoints.AuthDevice;
using Xunit;

namespace Initium.Portal.Tests.Web.ApiEndpoints.AuthDevice
{
    public class CompleteAuthDeviceRegistrationTests
    {
        public class EndpointRequestValidatorTests
            {
                [Fact]
                public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
                {
                    var request = new CompleteAuthDeviceRegistration.EndpointRequest
                    {
                        Name = "name",
                    };
                    var validator = new CompleteAuthDeviceRegistration.EndpointRequestValidator();
                    var result = validator.Validate(request);
                    Assert.True(result.IsValid);
                }

                [Fact]
                public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
                {
                    var request = new CompleteAuthDeviceRegistration.EndpointRequest
                    {
                        Name = string.Empty,
                    };
                    var validator = new CompleteAuthDeviceRegistration.EndpointRequestValidator();
                    var result = validator.Validate(request);
                    Assert.False(result.IsValid);
                    Assert.Contains(
                        result.Errors,
                        failure => failure.PropertyName == "Name");
                }

                [Fact]
                public void Validate_GivenPasswordIsNull_ExpectValidationFailure()
                {
                    var request = new CompleteAuthDeviceRegistration.EndpointRequest
                    {
                        Name = null,
                    };
                    var validator = new CompleteAuthDeviceRegistration.EndpointRequestValidator();
                    var result = validator.Validate(request);
                    Assert.False(result.IsValid);
                    Assert.Contains(
                        result.Errors,
                        failure => failure.PropertyName == "Name");
                }
            }
    }
}