// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Controllers.Api.AuthEmail;
using Stance.Web.Controllers.Api.AuthEmail.Models;
using Xunit;

namespace Stance.Tests.Web.Controllers.Api.AuthEmail
{
    public class AuthEmailApiControllerTests
    {
        [Fact]
        public async Task RevokeDevice_GivenCommandExecutes_ExpectSuccessfulJsonResult()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.Send(
                    It.IsAny<EmailMfaRequestedCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ResultWithError.Ok<ErrorData>());

            var controller = new AuthEmailApiController(mediator.Object);

            var response = await controller.RequestMfaEmail();

            var result = Assert.IsType<JsonResult>(response);
            var options = Assert.IsType<RequestMfaEmailResponse>(result.Value);
            Assert.True(options.Success);
        }

        [Fact]
        public async Task RevokeDevice_GivenCommandFailsToExecute_ExpectFailedJsonResult()
        {
            var mediator = new Mock<IMediator>();
            mediator
                .Setup(x => x.Send(
                    It.IsAny<EmailMfaRequestedCommand>(),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));

            var controller = new AuthEmailApiController(mediator.Object);

            var response = await controller.RequestMfaEmail();

            var result = Assert.IsType<JsonResult>(response);
            var options = Assert.IsType<RequestMfaEmailResponse>(result.Value);
            Assert.False(options.Success);
        }
    }
}