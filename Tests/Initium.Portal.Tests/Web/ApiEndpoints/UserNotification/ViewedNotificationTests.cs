// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Contracts;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.ApiEndpoints;
using Initium.Portal.Web.ApiEndpoints.UserNotification;
using MaybeMonad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.ApiEndpoints.UserNotification
{
    public class ViewedNotificationTests
    {
        [Fact]
        public async Task HandleAsync_GivenFailedExecution_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<MarkNotificationAsViewedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var endpoint =
                new ViewedNotification(currentAuthenticatedUserProvider.Object, mediator.Object);

            var response = await endpoint.HandleAsync(new ViewedNotification.EndpointRequest
            {
                NotificationId = TestVariables.NotificationId,
            });
            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<BasicEndpointResponse>(rawResult.Value);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task HandleAsync_GivenNoUserIsAuthenticated_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe<ISystemUser>.Nothing);

            var endpoint =
                new ViewedNotification(currentAuthenticatedUserProvider.Object, mediator.Object);

            var response = await endpoint.HandleAsync(new ViewedNotification.EndpointRequest());
            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<BasicEndpointResponse>(rawResult.Value);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task HandleAsync_GivenRequestIsNotValid_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(new Mock<ISystemUser>().Object));

            var endpoint =
                new ViewedNotification(currentAuthenticatedUserProvider.Object, mediator.Object);
            endpoint.ModelState.AddModelError("key", "error-message");

            var response = await endpoint.HandleAsync(new ViewedNotification.EndpointRequest());
            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<BasicEndpointResponse>(rawResult.Value);
            Assert.False(result.IsSuccess);
        }

        [Fact]
        public async Task HandleAsync_GivenSuccessfulExecution_ExpectResponseWithFailedStatus()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<MarkNotificationAsViewedCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());
            var currentAuthenticatedUserProvider = new Mock<ICurrentAuthenticatedUserProvider>();
            var authenticatedUser = new Mock<ISystemUser>();
            authenticatedUser.Setup(x => x.UserId).Returns(TestVariables.AuthenticatedUserId);
            currentAuthenticatedUserProvider.Setup(x => x.CurrentAuthenticatedUser)
                .Returns(Maybe.From(authenticatedUser.Object));

            var endpoint =
                new ViewedNotification(currentAuthenticatedUserProvider.Object, mediator.Object);

            var response = await endpoint.HandleAsync(new ViewedNotification.EndpointRequest
            {
                NotificationId = TestVariables.NotificationId,
            });
            var rawResult = Assert.IsType<OkObjectResult>(response.Result);
            var result = Assert.IsType<BasicEndpointResponse>(rawResult.Value);
            Assert.True(result.IsSuccess);
        }

        public class EndpointRequestValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var request = new ViewedNotification.EndpointRequest
                {
                    NotificationId = TestVariables.NotificationId,
                };
                var validator = new ViewedNotification.EndpointRequestValidator();
                var result = validator.Validate(request);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenNotificationIdIsEmpty_ExpectValidationFailure()
            {
                var request = new ViewedNotification.EndpointRequest
                {
                    NotificationId = Guid.Empty,
                };
                var validator = new ViewedNotification.EndpointRequestValidator();
                var result = validator.Validate(request);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "NotificationId");
            }
        }
    }
}