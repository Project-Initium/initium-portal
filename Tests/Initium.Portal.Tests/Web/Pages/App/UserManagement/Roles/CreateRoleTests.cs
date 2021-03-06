﻿// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.CommandResults.RoleAggregate;
using Initium.Portal.Domain.Commands.RoleAggregate;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Pages.App.UserManagement.Roles;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.App.UserManagement.Roles
{
    public class CreateRoleTests
    {
        [Fact]
        public async Task OnPostAsync_GivenInvalidModelState_ExpectRedirectToPageResultToSelfAndNoCommandExecuted()
        {
            var mediator = new Mock<IMediator>();

            var page = new CreateRole(mediator.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPostAsync();
            mediator.Verify(x => x.Send(It.IsAny<CreateRoleCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_GivenValidModelStateAndFailedResult_ExpectRedirectToPageResultToSelfAndPrgSet()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CreateRoleCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(() =>
                Result.Fail<CreateRoleCommandResult, ErrorData>(new ErrorData(ErrorCodes.SavingChanges)));
            var page = new CreateRole(mediator.Object) { PageModel = new CreateRole.Model() };

            var result = await page.OnPostAsync();
            Assert.Equal(PrgState.Failed, page.PrgState);
            var actualResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.True(string.IsNullOrWhiteSpace(actualResult.PageName));
        }

        [Fact]
        public async Task
            OnPostAsync_GivenValidModelStateAndSuccessfulResult_ExpectRedirectToPageResultToViewPageAndIdSet()
        {
            var roleId = Guid.NewGuid();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CreateRoleCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(() =>
                Result.Ok<CreateRoleCommandResult, ErrorData>(new CreateRoleCommandResult(roleId)));
            var page = new CreateRole(mediator.Object) { PageModel = new CreateRole.Model() };

            var result = await page.OnPostAsync();
            var actualResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(CorePageLocations.RoleView, actualResult.PageName);
            Assert.Equal(roleId, actualResult.RouteValues["id"]);
        }

        public class ValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var model = new CreateRole.Model { Name = "name" };
                var validator = new CreateRole.Validator();
                var result = validator.Validate(model);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
            {
                var model = new CreateRole.Model { Name = string.Empty };
                var validator = new CreateRole.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "Name");
            }

            [Fact]
            public void Validate_GivenNameIsNull_ExpectValidationFailure()
            {
                var model = new CreateRole.Model { Name = null };
                var validator = new CreateRole.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "Name");
            }
        }
    }
}