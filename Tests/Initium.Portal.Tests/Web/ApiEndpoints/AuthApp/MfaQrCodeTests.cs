// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using Initium.Portal.Web.ApiEndpoints.AuthApp;
using Microsoft.AspNetCore.Mvc;
using Xunit;

namespace Initium.Portal.Tests.Web.ApiEndpoints.AuthApp
{
    public class MfaQrCodeTests
    {
        [Fact]
        public async Task HandleAsync_GivenInvalidModelState_ExpectFailedResult()
        {
            var endpoint = new MfaQrCode();

            endpoint.ModelState.AddModelError("key", "error-message");
            var response = await endpoint.HandleAsync(null);
            Assert.IsAssignableFrom<NotFoundResult>(response.Result);
        }

        [Fact]
        public async Task HandleAsync_GivenValidModelState_ExpectFileResult()
        {
            var endpoint = new MfaQrCode();

            var response = await endpoint.HandleAsync(
                new MfaQrCode.EndpointRequest
            {
                AuthenticatorUri = "authenticator-uri",
            });

            Assert.IsAssignableFrom<FileResult>(response.Result);
        }

        public class EndpointRequestValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var request = new MfaQrCode.EndpointRequest()
                {
                    AuthenticatorUri = "authenticator-uri",
                };
                var validator = new MfaQrCode.EndpointRequestValidator();
                var result = validator.Validate(request);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenAuthenticatorUriIsEmpty_ExpectValidationFailure()
            {
                var request = new MfaQrCode.EndpointRequest
                {
                    AuthenticatorUri = string.Empty,
                };
                var validator = new MfaQrCode.EndpointRequestValidator();
                var result = validator.Validate(request);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "AuthenticatorUri");
            }

            [Fact]
            public void Validate_GivenAuthenticatorUriIsNull_ExpectValidationFailure()
            {
                var request = new MfaQrCode.EndpointRequest();
                var validator = new MfaQrCode.EndpointRequestValidator();
                var result = validator.Validate(request);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "AuthenticatorUri");
            }
        }
    }
}