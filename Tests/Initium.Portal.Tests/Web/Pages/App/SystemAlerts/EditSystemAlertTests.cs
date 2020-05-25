// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Constants;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.Commands.SystemAlertAggregate;
using Initium.Portal.Queries.Contracts.Static;
using Initium.Portal.Queries.Static.Models.Messaging;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Pages.App.SystemAlerts;
using MaybeMonad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.App.SystemAlerts
{
    public class EditSystemAlertTests
    {
        [Fact]
        public async Task OnGetAsync_GivenNoSystemAlertFound_ExpectNotFoundResult()
        {
            var mediator = new Mock<IMediator>();

            var messagingQueries = new Mock<IMessagingQueries>();
            messagingQueries.Setup(x => x.GetDetailedSystemAlertById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe<DetailedSystemAlert>.Nothing);

            var page = new EditSystemAlert(mediator.Object, messagingQueries.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_GivenPageModelIsNull_ExpectPageModelToBeSetFromDatabaseAndPageResultReturned()
        {
            var mediator = new Mock<IMediator>();

            var detailedSystemAlert = new DetailedSystemAlert(
                TestVariables.SystemAlertId,
                "name",
                "message",
                SystemAlertType.Critical,
                TestVariables.Now,
                TestVariables.Now.AddHours(1));
            var messagingQueries = new Mock<IMessagingQueries>();
            messagingQueries.Setup(x => x.GetDetailedSystemAlertById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe.From(detailedSystemAlert));

            var page = new EditSystemAlert(mediator.Object, messagingQueries.Object);
            Assert.IsType<PageResult>(await page.OnGetAsync());
            Assert.Equal(TestVariables.SystemAlertId, page.PageModel.SystemAlertId);
            Assert.Equal("name", page.PageModel.Name);
            Assert.Equal("message", page.PageModel.Message);
            Assert.Equal(SystemAlertType.Critical, page.PageModel.Type);
            Assert.Equal(TestVariables.Now, page.PageModel.WhenToShow);
            Assert.Equal(TestVariables.Now.AddHours(1), page.PageModel.WhenToHide);
            Assert.Equal("name", page.Name);
        }

        [Fact]
        public async Task OnGetAsync_GivenPageModelIsNotNull_ExpectPageModelNotToBeOverridenAndPageResultReturned()
        {
            var mediator = new Mock<IMediator>();

            var detailedSystemAlert = new DetailedSystemAlert(
                TestVariables.SystemAlertId,
                "name",
                "message",
                SystemAlertType.Critical,
                TestVariables.Now,
                TestVariables.Now.AddHours(1));
            var messagingQueries = new Mock<IMessagingQueries>();
            messagingQueries.Setup(x => x.GetDetailedSystemAlertById(It.IsAny<Guid>()))
                .ReturnsAsync(Maybe.From(detailedSystemAlert));

            var page = new EditSystemAlert(mediator.Object, messagingQueries.Object)
            {
                PageModel = new EditSystemAlert.Model
                {
                    Message = "message-1",
                    Name = "name-1",
                    SystemAlertId = TestVariables.SystemAlertId,
                    Type = SystemAlertType.High,
                    WhenToHide = TestVariables.Now.AddHours(2),
                    WhenToShow = TestVariables.Now.AddHours(-2),
                },
            };
            Assert.IsType<PageResult>(await page.OnGetAsync());
            Assert.Equal(TestVariables.SystemAlertId, page.PageModel.SystemAlertId);
            Assert.Equal("name-1", page.PageModel.Name);
            Assert.Equal("message-1", page.PageModel.Message);
            Assert.Equal(SystemAlertType.High, page.PageModel.Type);
            Assert.Equal(TestVariables.Now.AddHours(-2), page.PageModel.WhenToShow);
            Assert.Equal(TestVariables.Now.AddHours(2), page.PageModel.WhenToHide);
            Assert.Equal("name", page.Name);
        }

        [Fact]
        public async Task OnPostAsync_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();
            var messagingQueries = new Mock<IMessagingQueries>();

            var page = new EditSystemAlert(mediator.Object, messagingQueries.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_GivenCommandExecutesSuccessfully_ExpectRedirectToPageResultAndSuccessfulPrgState()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UpdateSystemAlertCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ResultWithError.Ok<ErrorData>());
            var messagingQueries = new Mock<IMessagingQueries>();

            var page = new EditSystemAlert(mediator.Object, messagingQueries.Object)
            {
                PageModel = new EditSystemAlert.Model
                {
                    SystemAlertId = TestVariables.SystemAlertId,
                },
            };

            var result = await page.OnPostAsync();
            var pageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Success, page.PrgState);
            Assert.Single(page.PageNotifications, x => x.PageNotificationType == NotificationPageModel.PageNotification.Success);
            Assert.Equal(TestVariables.SystemAlertId, pageResult.RouteValues["id"]);
            Assert.Equal(PageLocations.SystemAlertView, pageResult.PageName);
        }

        [Fact]
        public async Task OnPostAsync_GivenCommandFailsToExecute_ExpectRedirectToPageResultAndFailedPrgState()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UpdateSystemAlertCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));
            var messagingQueries = new Mock<IMessagingQueries>();

            var page = new EditSystemAlert(mediator.Object, messagingQueries.Object)
            {
                PageModel = new EditSystemAlert.Model(),
            };

            var result = await page.OnPostAsync();
            var pageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Failed, page.PrgState);
            Assert.Single(page.PageNotifications, x => x.PageNotificationType == NotificationPageModel.PageNotification.Error);
            Assert.Null(pageResult.PageName);
        }

        public class ValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var cmd = new EditSystemAlert.Model { Name = "name", Message = "message", SystemAlertId = TestVariables.SystemAlertId };
                var validator = new EditSystemAlert.Validator();
                var result = validator.Validate(cmd);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
            {
                var cmd = new EditSystemAlert.Model { Name = string.Empty, Message = "message", SystemAlertId = TestVariables.SystemAlertId };
                var validator = new EditSystemAlert.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Name");
            }

            [Fact]
            public void Validate_GivenNameIsNull_ExpectValidationFailure()
            {
                var cmd = new EditSystemAlert.Model { Name = null, Message = "message", SystemAlertId = TestVariables.SystemAlertId };
                var validator = new EditSystemAlert.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Name");
            }

            [Fact]
            public void Validate_GivenMessageIsEmpty_ExpectValidationFailure()
            {
                var cmd = new EditSystemAlert.Model { Name = "name", Message = string.Empty, SystemAlertId = TestVariables.SystemAlertId };
                var validator = new EditSystemAlert.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Message");
            }

            [Fact]
            public void Validate_GiveMessageIsNull_ExpectValidationFailure()
            {
                var cmd = new EditSystemAlert.Model { Name = "name", Message = null, SystemAlertId = TestVariables.SystemAlertId };
                var validator = new EditSystemAlert.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Message");
            }

            [Fact]
            public void Validate_GivenSystemAlertIdIsEmpty_ExpectValidationFailure()
            {
                var cmd = new EditSystemAlert.Model { Name = "name", Message = "message", SystemAlertId = Guid.Empty };
                var validator = new EditSystemAlert.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "SystemAlertId");
            }
        }
    }
}