// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.ApiEndpoints;
using Initium.Portal.Web.ApiEndpoints.AuthApp;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.ApiEndpoints.AuthApp
{
    public class EnrollAuthAppTests
    {
        [Fact]
        public async Task HandleAsync_GivenExecutionFails_ExpectFailedResult()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EnrollAuthenticatorAppCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));

            var endpoint = new EnrollAuthApp(mediator.Object);

            var response = await endpoint.HandleAsync(new EnrollAuthApp.EndpointRequest());
            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<BasicEndpointResponse>(rawResult.Value);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task HandleAsync_GivenExecutionSucceeds_ExpectSuccessfulResult()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EnrollAuthenticatorAppCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ResultWithError.Ok<ErrorData>());

            var endpoint = new EnrollAuthApp(mediator.Object);

            var response = await endpoint.HandleAsync(new EnrollAuthApp.EndpointRequest());
            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<BasicEndpointResponse>(rawResult.Value);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task HandleAsync_GivenInvalidModelState_ExpectFailedResult()
        {
            var mediator = new Mock<IMediator>();

            var endpoint = new EnrollAuthApp(mediator.Object);

            endpoint.ModelState.AddModelError("key", "error-message");
            var response = await endpoint.HandleAsync(null);
            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<BasicEndpointResponse>(rawResult.Value);
            Assert.False(result.IsSuccess);
        }

        public class EndpointRequestValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var request = new EnrollAuthApp.EndpointRequest
                {
                    Code = "code",
                    SharedKey = "shared-key",
                };
                var validator = new EnrollAuthApp.EndpointRequestValidator();
                var result = validator.Validate(request);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenCodeIsEmpty_ExpectValidationFailure()
            {
                var request = new EnrollAuthApp.EndpointRequest
                {
                    Code = string.Empty,
                    SharedKey = "shared-key",
                };
                var validator = new EnrollAuthApp.EndpointRequestValidator();
                var result = validator.Validate(request);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Code");
            }

            [Fact]
            public void Validate_GivenCodeIsNull_ExpectValidationFailure()
            {
                var request = new EnrollAuthApp.EndpointRequest
                {
                    Code = null,
                    SharedKey = "shared-key",
                };
                var validator = new EnrollAuthApp.EndpointRequestValidator();
                var result = validator.Validate(request);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Code");
            }

            [Fact]
            public void Validate_GivenSharedKeyIsEmpty_ExpectValidationFailure()
            {
                var request = new EnrollAuthApp.EndpointRequest
                {
                    Code = "code",
                    SharedKey = string.Empty,
                };
                var validator = new EnrollAuthApp.EndpointRequestValidator();
                var result = validator.Validate(request);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "SharedKey");
            }

            [Fact]
            public void Validate_GivenSharedKeyIsNull_ExpectValidationFailure()
            {
                var request = new EnrollAuthApp.EndpointRequest
                {
                    Code = "code",
                    SharedKey = null,
                };
                var validator = new EnrollAuthApp.EndpointRequestValidator();
                var result = validator.Validate(request);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "SharedKey");
            }
        }
    }
}