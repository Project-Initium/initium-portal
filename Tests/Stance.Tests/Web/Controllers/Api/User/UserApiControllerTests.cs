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
using Stance.Web.Controllers.Api.User;
using Xunit;

namespace Stance.Tests.Web.Controllers.Api.User
{
    public class UserApiControllerTests
    {
        [Fact]
        public async Task UnlockAccount_GivenSuccessfulExecution_ExpectSuccessfulResponse()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UnlockAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());
            var controller = new UserApiController(mediator.Object);

            var result = Assert.IsType<JsonResult>(await controller.UnlockAccount(new UnlockAccountRequest()));
            var responseModel = Assert.IsType<UnlockAccountResponse>(result.Value);
            Assert.True(responseModel.IsSuccess);
        }

        [Fact]
        public async Task UnlockAccount_GivenFailedExecution_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UnlockAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var controller = new UserApiController(mediator.Object);

            var result = Assert.IsType<JsonResult>(await controller.UnlockAccount(new UnlockAccountRequest()));
            var responseModel = Assert.IsType<UnlockAccountResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }
    }
}