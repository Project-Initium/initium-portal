﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.Controllers.Api;
using Initium.Portal.Web.Controllers.Api.AuthEmail;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Controllers.Api.AuthEmail
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
            var options = Assert.IsType<BasicApiResponse>(result.Value);
            Assert.True(options.IsSuccess);
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
            var options = Assert.IsType<BasicApiResponse>(result.Value);
            Assert.False(options.IsSuccess);
        }
    }
}