// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.ApiEndpoints;
using Initium.Portal.Web.ApiEndpoints.AuthDevice;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.ApiEndpoints.AuthDevice
{
    public class RevokeDeviceTests
    {
        [Fact]
        public async Task HandleAsync_GivenCommandExecutes_ExpectSuccessfulJsonResult()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.Send(
                    It.IsAny<RevokeAuthenticatorDeviceCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ResultWithError.Ok<ErrorData>());

            var endpoint = new RevokeDevice(mediator.Object);

            var response = await endpoint.HandleAsync(new RevokeDevice.EndpointRequest());

            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<BasicEndpointResponse>(rawResult.Value);
            Assert.True(result.IsSuccess);
        }

        [Fact]
        public async Task HandleAsync_GivenCommandFailsToExecute_ExpectFailedJsonResult()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.Send(
                    It.IsAny<RevokeAuthenticatorDeviceCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));

            var endpoint = new RevokeDevice(mediator.Object);

            var response = await endpoint.HandleAsync(new RevokeDevice.EndpointRequest());

            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<BasicEndpointResponse>(rawResult.Value);
            Assert.False(result.IsSuccess);
        }
    }
}