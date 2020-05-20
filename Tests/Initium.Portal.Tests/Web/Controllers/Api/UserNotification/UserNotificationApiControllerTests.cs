using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.Controllers.Api;
using Initium.Portal.Web.Controllers.Api.User.Models;
using Initium.Portal.Web.Controllers.Api.UserNotification;
using Initium.Portal.Web.Controllers.Api.UserNotification.Models;
using MaybeMonad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Controllers.Api.UserNotification
{
    public class UserNotificationApiControllerTests
    {
        [Fact]
        public async Task ViewedNotification_GivenRequestIsNotValid_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new Mock<ISystemUser>().Object));
            var controller = new UserNotificationApiController(mediator.Object, currentAuthenticatedUserProvider.Object);
            controller.ModelState.AddModelError("key", "error-message");

            var result = Assert.IsType<JsonResult>(await controller.ViewedNotification(new ViewedNotificationRequest()));
            var responseModel = Assert.IsType<BasicApiResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }

        [Fact]
        public async Task ViewedNotification_GivenNoUserIsAuthenticated_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);
            var controller = new UserNotificationApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.ViewedNotification(new ViewedNotificationRequest()));
            var responseModel = Assert.IsType<BasicApiResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }
        
        [Fact]
        public async Task ViewedNotification_GivenSuccessfulExecution_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<MarkNotificationAsViewedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var controller = new UserNotificationApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.ViewedNotification(new ViewedNotificationRequest
            {
                NotificationId = TestVariables.NotificationId,
            }));
            var responseModel = Assert.IsType<BasicApiResponse>(result.Value);
            Assert.True(responseModel.IsSuccess);
        }
        
        [Fact]
        public async Task ViewedNotification_GivenFailedExecution_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<MarkNotificationAsViewedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var controller = new UserNotificationApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.ViewedNotification(new ViewedNotificationRequest
            {
                NotificationId = TestVariables.NotificationId,
            }));
            var responseModel = Assert.IsType<BasicApiResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }
        
        [Fact]
        public async Task ViewedAllNotifications_GivenNoUserIsAuthenticated_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);
            var controller = new UserNotificationApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.ViewedAllNotifications());
            var responseModel = Assert.IsType<BasicApiResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }

        [Fact]
        public async Task ViewedAllNotifications_GivenSuccessfulExecution_ExpectSuccessfulResponse()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<MarkAllUnreadNotificationsAsViewedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var controller = new UserNotificationApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.ViewedAllNotifications());
            var responseModel = Assert.IsType<BasicApiResponse>(result.Value);
            Assert.True(responseModel.IsSuccess);
        }
        
        [Fact]
        public async Task ViewedAllNotifications_GivenFailedExecution_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<MarkAllUnreadNotificationsAsViewedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var controller = new UserNotificationApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.ViewedAllNotifications());
            var responseModel = Assert.IsType<BasicApiResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }
        
        [Fact]
        public async Task DismissedNotification_GivenRequestIsNotValid_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new Mock<ISystemUser>().Object));
            var controller = new UserNotificationApiController(mediator.Object, currentAuthenticatedUserProvider.Object);
            controller.ModelState.AddModelError("key", "error-message");

            var result = Assert.IsType<JsonResult>(await controller.DismissedNotification(new DismissedNotificationRequest()));
            var responseModel = Assert.IsType<BasicApiResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }
        
        [Fact]
        public async Task DismissedNotification_GivenNoUserIsAuthenticated_ExpectResponseWithFailedStatu()
        {
            var mediator = new Mock<IMediator>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);
            var controller = new UserNotificationApiController(mediator.Object, currentAuthenticatedUserProvider.Object);
            
            var result = Assert.IsType<JsonResult>(await controller.DismissedNotification(new DismissedNotificationRequest()));
            var responseModel = Assert.IsType<BasicApiResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }

        [Fact]
        public async Task DismissedNotification_GivenSuccessfulExecution_ExpectSuccessfulResponse()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<MarkNotificationAsDismissedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var controller = new UserNotificationApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.DismissedNotification(new DismissedNotificationRequest
            {
                NotificationId = TestVariables.NotificationId,
            }));
            var responseModel = Assert.IsType<BasicApiResponse>(result.Value);
            Assert.True(responseModel.IsSuccess);
        }
        
        [Fact]
        public async Task DismissedNotification_GivenFailedExecution_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<MarkNotificationAsDismissedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var controller = new UserNotificationApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.DismissedNotification(new DismissedNotificationRequest
            {
                NotificationId = TestVariables.NotificationId,
            }));
            var responseModel = Assert.IsType<BasicApiResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }
        
        [Fact]
        public async Task DismissedAllNotification_GivenNoUserIsAuthenticated_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);
            var controller = new UserNotificationApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.DismissedAllNotification());
            var responseModel = Assert.IsType<BasicApiResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }

        [Fact]
        public async Task DismissedAllNotification_GivenSuccessfulExecution_ExpectSuccessfulResponse()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<MarkAllRetainedNotificationsAsDismissedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var controller = new UserNotificationApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.DismissedAllNotification());
            var responseModel = Assert.IsType<BasicApiResponse>(result.Value);
            Assert.True(responseModel.IsSuccess);
        }
        
        [Fact]
        public async Task DismissedAllNotification_GivenFailedExecution_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<MarkAllRetainedNotificationsAsDismissedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var controller = new UserNotificationApiController(mediator.Object, currentAuthenticatedUserProvider.Object);

            var result = Assert.IsType<JsonResult>(await controller.DismissedAllNotification());
            var responseModel = Assert.IsType<BasicApiResponse>(result.Value);
            Assert.False(responseModel.IsSuccess);
        }
    }
}