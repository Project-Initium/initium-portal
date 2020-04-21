// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Fido2NetLib;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.CommandResults.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Controllers.Api.AuthDevice;
using Stance.Web.Infrastructure.Contracts;
using Xunit;

namespace Stance.Tests.Web.Controllers.Api.AuthDevice
{
    public class AuthDeviceApiControllerTests
    {
        [Fact]
        public async Task
            InitialAuthDeviceRegistration_GivenResultIsNotSuccessful_ExpectCredentialCreateOptionsWithErrorStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.Send(
                    It.IsAny<InitiateAuthenticatorDeviceEnrollmentCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    Result.Fail<InitiateAuthenticatorDeviceEnrollmentCommandResult, ErrorData>(
                        new ErrorData(ErrorCodes.AuthenticationFailed)));
            var authenticationService = new Mock<IAuthenticationService>();

            var controller = new AuthDeviceApiController(mediator.Object, authenticationService.Object);
            var tempDataDictionary = new Mock<ITempDataDictionary>();

            controller.TempData = tempDataDictionary.Object;

            var response = await controller.InitialAuthDeviceRegistration(new InitialAuthDeviceRegistrationRequest());

            var result = Assert.IsType<JsonResult>(response);
            var options = Assert.IsType<CredentialCreateOptions>(result.Value);
            Assert.Equal("error", options.Status);
        }

        [Fact]
        public async Task
            InitialAuthDeviceRegistration_GivenResultIsSuccessful_ExpectCredentialCreateOptionsWithOptionsSet()
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
            var authenticationService = new Mock<IAuthenticationService>();

            var controller = new AuthDeviceApiController(mediator.Object, authenticationService.Object);
            var tempDataDictionary = new Mock<ITempDataDictionary>();
            controller.TempData = tempDataDictionary.Object;

            var response = await controller.InitialAuthDeviceRegistration(new InitialAuthDeviceRegistrationRequest());

            var result = Assert.IsType<JsonResult>(response);
            var options = Assert.IsType<CredentialCreateOptions>(result.Value);
            Assert.Equal("ok", options.Status);
        }

        [Fact]
        public async Task RevokeDevice_GivenCommandExecutes_ExpectSuccessfulJsonResult()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.Send(
                    It.IsAny<RevokeAuthenticatorDeviceCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ResultWithError.Ok<ErrorData>());
            var authenticationService = new Mock<IAuthenticationService>();

            var controller = new AuthDeviceApiController(mediator.Object, authenticationService.Object);

            var response = await controller.RevokeDevice(new RevokeDeviceRequest());

            var result = Assert.IsType<JsonResult>(response);
            var options = Assert.IsType<RevokeDeviceResponse>(result.Value);
            Assert.True(options.IsSuccess);
        }

        [Fact]
        public async Task RevokeDevice_GivenCommandFailsToExecute_ExpectFailedJsonResult()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.Send(
                    It.IsAny<RevokeAuthenticatorDeviceCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var authenticationService = new Mock<IAuthenticationService>();

            var controller = new AuthDeviceApiController(mediator.Object, authenticationService.Object);

            var response = await controller.RevokeDevice(new RevokeDeviceRequest());

            var result = Assert.IsType<JsonResult>(response);
            var options = Assert.IsType<RevokeDeviceResponse>(result.Value);
            Assert.False(options.IsSuccess);
        }
    }
}