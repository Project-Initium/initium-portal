// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Fido2NetLib;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.CommandResults.UserAggregate;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.ApiEndpoints.AuthDevice;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.ApiEndpoints.AuthDevice
{
    public class InitialAuthDeviceRegistrationTests
    {
        [Fact]
        public async Task
            HandleAsync_GivenResultIsNotSuccessful_ExpectCredentialCreateOptionsWithErrorStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.Send(
                    It.IsAny<InitiateAuthenticatorDeviceEnrollmentCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    Result.Fail<InitiateAuthenticatorDeviceEnrollmentCommandResult, ErrorData>(
                        new ErrorData(ErrorCodes.AuthenticationFailed)));
            var tempDataDictionaryFactory = new Mock<ITempDataDictionaryFactory>();

            var endpoint = new InitialAuthDeviceRegistration(mediator.Object, tempDataDictionaryFactory.Object);

            var response = await endpoint.HandleAsync(new InitialAuthDeviceRegistration.EndpointRequest());

            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<CredentialCreateOptions>(rawResult.Value);
            Assert.Equal("error", result.Status);
        }

        [Fact]
        public async Task
            HandleAsync_GivenResultIsSuccessful_ExpectCredentialCreateOptionsWithOptionsSet()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.Send(
                    It.IsAny<InitiateAuthenticatorDeviceEnrollmentCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    Result.Ok<InitiateAuthenticatorDeviceEnrollmentCommandResult, ErrorData>(
                        new InitiateAuthenticatorDeviceEnrollmentCommandResult(new CredentialCreateOptions()
                        {
                            Status = "ok",
                        })));
            var tempDataDictionaryFactory = new Mock<ITempDataDictionaryFactory>();
            tempDataDictionaryFactory.Setup(x => x.GetTempData(It.IsAny<HttpContext>()))
                .Returns(new Mock<ITempDataDictionary>().Object);

            var endpoint = new InitialAuthDeviceRegistration(mediator.Object, tempDataDictionaryFactory.Object);

            var response = await endpoint.HandleAsync(new InitialAuthDeviceRegistration.EndpointRequest());

            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<CredentialCreateOptions>(rawResult.Value);
            Assert.Equal("ok", result.Status);
        }
    }
}