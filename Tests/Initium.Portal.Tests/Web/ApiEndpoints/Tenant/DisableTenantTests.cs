// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.Commands.TenantAggregate;
using Initium.Portal.Web.ApiEndpoints;
using Initium.Portal.Web.ApiEndpoints.Tenant;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.ApiEndpoints.Tenant
{
    public class DisableTenantTests
    {
        [Fact]
        public async Task HandleAsync_GivenInvalidModelState_ExpectFailedResult()
        {
            var mediator = new Mock<IMediator>();

            var endpoint = new DisableTenant(mediator.Object);

            endpoint.ModelState.AddModelError("key", "error-message");
            var response = await endpoint.HandleAsync(null);
            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<BasicEndpointResponse>(rawResult.Value);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task HandleAsync_GivenExecutionSucceeds_ExpectSuccessfulResult()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DisableTenantCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());

            var endpoint = new DisableTenant(mediator.Object);

            var response = await endpoint.HandleAsync(new DisableTenant.EndpointRequest());
            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<BasicEndpointResponse>(rawResult.Value);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task HandleAsync_GivenExecutionFails_ExpectFailedResult()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DisableTenantCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));

            var endpoint = new DisableTenant(mediator.Object);

            var response = await endpoint.HandleAsync(new DisableTenant.EndpointRequest());
            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<BasicEndpointResponse>(rawResult.Value);
            Assert.False(result.IsSuccess);
        }

        public class DisableTenantValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var request = new DisableTenant.EndpointRequest()
                {
                    TenantId = TestVariables.TenantId,
                };
                var validator = new DisableTenant.EndpointRequestValidator();
                var result = validator.Validate(request);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
            {
                var request = new DisableTenant.EndpointRequest()
                {
                    TenantId = Guid.Empty,
                };
                var validator = new DisableTenant.EndpointRequestValidator();
                var result = validator.Validate(request);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "TenantId");
            }
        }
    }
}