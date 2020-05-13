// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.Controllers.Api.User;
using Initium.Portal.Web.Controllers.Api.User.Models;
using MaybeMonad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Controllers.Api.User
{
    public class UserApiControllerTests
    {
        [Fact]
        public async Task UnlockAccount_GivenRequestIsNotValid_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new Mock<ISystemUser>().Object));
            var controller = new UserApiController(mediator.Object, currentAuthenticatedUserProvider.Object);
            controller.ModelState.AddModelError("key", "error-message");

            var result = Assert.IsType<JsonResult>(await controller.UnlockAccount(new UnlockAccountRequest()));
            var responseModel = Assert.IsType<UnlockAccountResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }

        [Fact]
        public async Task UnlockAccount_GivenSuccessfulExecution_ExpectSuccessfulResponse()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UnlockAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var controller = new UserApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.UnlockAccount(new UnlockAccountRequest
            {
                UserId = TestVariables.UserId,
            }));
            var responseModel = Assert.IsType<UnlockAccountResponse>(result.Value);
            Assert.True(responseModel.IsSuccess);
        }

        [Fact]
        public async Task UnlockAccount_GivenAttemptToUnlockSelf_ExpectFailedResponse()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UnlockAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var controller = new UserApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.UnlockAccount(new UnlockAccountRequest
            {
                UserId = TestVariables.AuthenticatedUserId,
            }));
            var responseModel = Assert.IsType<UnlockAccountResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }

        [Fact]
        public async Task UnlockAccount_GivenNoAuthenticatedUser_ExpectFailedResponse()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UnlockAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);

            var controller = new UserApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.UnlockAccount(new UnlockAccountRequest
            {
                UserId = TestVariables.AuthenticatedUserId,
            }));
            var responseModel = Assert.IsType<UnlockAccountResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }

        [Fact]
        public async Task UnlockAccount_GivenFailedExecution_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UnlockAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new Mock<ISystemUser>().Object));
            var controller = new UserApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.UnlockAccount(new UnlockAccountRequest()));
            var responseModel = Assert.IsType<UnlockAccountResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }

        [Fact]
        public async Task DisableAccount_GivenRequestIsNotValid_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new Mock<ISystemUser>().Object));
            var controller = new UserApiController(mediator.Object, currentAuthenticatedUserProvider.Object);
            controller.ModelState.AddModelError("key", "error-message");

            var result = Assert.IsType<JsonResult>(await controller.DisableAccount(new DisableAccountRequest()));
            var responseModel = Assert.IsType<DisableAccountResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }

        [Fact]
        public async Task DisableAccount_GivenSuccessfulExecution_ExpectSuccessfulResponse()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DisableAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var controller = new UserApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.DisableAccount(new DisableAccountRequest
            {
                UserId = TestVariables.UserId,
            }));
            var responseModel = Assert.IsType<DisableAccountResponse>(result.Value);
            Assert.True(responseModel.IsSuccess);
        }

        [Fact]
        public async Task DisableAccount_GivenFailedExecution_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DisableAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var controller = new UserApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.DisableAccount(new DisableAccountRequest
            {
                UserId = TestVariables.UserId,
            }));
            var responseModel = Assert.IsType<DisableAccountResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }

        [Fact]
        public async Task DisableAccount_GivenAttemptToDisableSelf_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DisableAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var controller = new UserApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.DisableAccount(new DisableAccountRequest
            {
                UserId = TestVariables.AuthenticatedUserId,
            }));
            var responseModel = Assert.IsType<DisableAccountResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }

        [Fact]
        public async Task DisableAccount_GivenNoAuthenticatedUser_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<DisableAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);

            var controller = new UserApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.DisableAccount(new DisableAccountRequest
            {
                UserId = TestVariables.UserId,
            }));
            var responseModel = Assert.IsType<DisableAccountResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }

        [Fact]
        public async Task EnableAccount_GivenRequestIsNotValid_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new Mock<ISystemUser>().Object));

            var controller = new UserApiController(mediator.Object, currentAuthenticatedUserProvider.Object);
            controller.ModelState.AddModelError("key", "error-message");

            var result = Assert.IsType<JsonResult>(await controller.EnableAccount(new EnableAccountRequest()));
            var responseModel = Assert.IsType<EnableAccountResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }

        [Fact]
        public async Task EnableAccount_GivenSuccessfulExecution_ExpectSuccessfulResponse()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EnableAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var controller = new UserApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.EnableAccount(new EnableAccountRequest
            {
                UserId = TestVariables.UserId,
            }));
            var responseModel = Assert.IsType<EnableAccountResponse>(result.Value);
            Assert.True(responseModel.IsSuccess);
        }

        [Fact]
        public async Task EnableAccount_GivenFailedExecution_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EnableAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var controller = new UserApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.EnableAccount(new EnableAccountRequest
            {
                UserId = TestVariables.UserId,
            }));
            var responseModel = Assert.IsType<EnableAccountResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }

        [Fact]
        public async Task EnableAccount_GivenAttemptToEnableSelf_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EnableAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var controller = new UserApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.EnableAccount(new EnableAccountRequest
            {
                UserId = TestVariables.AuthenticatedUserId,
            }));
            var responseModel = Assert.IsType<EnableAccountResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }

        [Fact]
        public async Task EnableAccount_GivenNoAuthenticatedUser_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<EnableAccountCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);

            var controller = new UserApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.EnableAccount(new EnableAccountRequest
            {
                UserId = TestVariables.UserId,
            }));
            var responseModel = Assert.IsType<EnableAccountResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }
    }
}