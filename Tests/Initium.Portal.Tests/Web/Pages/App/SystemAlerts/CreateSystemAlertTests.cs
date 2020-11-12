// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.CommandResults.SystemAlertAggregate;
using Initium.Portal.Domain.Commands.SystemAlertAggregate;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Pages.App.SystemAlerts;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.App.SystemAlerts
{
    public class CreateSystemAlertTests
    {
        [Fact]
        public async Task OnPostAsync_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();

            var page = new CreateSystemAlert(mediator.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_GivenCommandExecutesSuccessfully_ExpectRedirectToPageResultAndSuccessfulPrgState()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CreateNewSystemAlertCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    Result.Ok<CreateNewSystemAlertCommandResult, ErrorData>(
                        new CreateNewSystemAlertCommandResult(TestVariables.SystemAlertId)));

            var page = new CreateSystemAlert(mediator.Object)
            {
                PageModel = new CreateSystemAlert.Model(),
            };

            var result = await page.OnPostAsync();
            var pageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Success, page.PrgState);
            Assert.Single(page.PageNotifications, x => x.PageNotificationType == NotificationPageModel.PageNotification.Success);
            Assert.Equal(TestVariables.SystemAlertId, pageResult.RouteValues["id"]);
            Assert.Equal(CorePageLocations.SystemAlertView, pageResult.PageName);
        }

        [Fact]
        public async Task OnPostAsync_GivenCommandFailsToExecute_ExpectRedirectToPageResultAndFailedPrgState()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CreateNewSystemAlertCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    Result.Fail<CreateNewSystemAlertCommandResult, ErrorData>(
                        new ErrorData(ErrorCodes.AuthenticationFailed)));

            var page = new CreateSystemAlert(mediator.Object)
            {
                PageModel = new CreateSystemAlert.Model(),
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
                var cmd = new CreateSystemAlert.Model { Name = "name", Message = "message" };
                var validator = new CreateSystemAlert.Validator();
                var result = validator.Validate(cmd);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
            {
                var cmd = new CreateSystemAlert.Model { Name = string.Empty, Message = "message" };
                var validator = new CreateSystemAlert.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Name");
            }

            [Fact]
            public void Validate_GivenNameIsNull_ExpectValidationFailure()
            {
                var cmd = new CreateSystemAlert.Model { Name = null, Message = "message" };
                var validator = new CreateSystemAlert.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Name");
            }

            [Fact]
            public void Validate_GivenMessageIsEmpty_ExpectValidationFailure()
            {
                var cmd = new CreateSystemAlert.Model { Name = "name", Message = string.Empty };
                var validator = new CreateSystemAlert.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Message");
            }

            [Fact]
            public void Validate_GiveMessageIsNull_ExpectValidationFailure()
            {
                var cmd = new CreateSystemAlert.Model { Name = "name", Message = null };
                var validator = new CreateSystemAlert.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Message");
            }
        }
    }
}