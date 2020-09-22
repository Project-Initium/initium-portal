// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.Commands.TenantAggregate;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models.Tenant;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Pages.App.Tenants;
using MaybeMonad;
using MediatR;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.App.Tenant
{
    public class EditTenantTests
    {
        [Fact]
        public async Task OnGetAsync_GivenNoSystemAlertFound_ExpectNotFoundResult()
        {
            var tenantQueryService = new Mock<ITenantQueryService>();
            tenantQueryService.Setup(x => x.GetTenantMetadataById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe<TenantMetadata>.Nothing);
            var mediator = new Mock<IMediator>();

            var page = new EditTenant(tenantQueryService.Object, mediator.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_GivenPageModelIsNull_ExpectPageModelToBeSetFromDatabaseAndPageResultReturned()
        {
            var tenantQueryService = new Mock<ITenantQueryService>();
            tenantQueryService.Setup(x => x.GetTenantMetadataById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(new TenantMetadata(
                    TestVariables.TenantId,
                    "identifier",
                    "name",
                    "connection-string",
                    null)));
            var mediator = new Mock<IMediator>();

            var page = new EditTenant(tenantQueryService.Object, mediator.Object);
            var tempDataDictionary = new Mock<ITempDataDictionary>();
            page.TempData = tempDataDictionary.Object;

            Assert.IsAssignableFrom<PageResult>(await page.OnGetAsync());
            Assert.Equal(TestVariables.TenantId, page.PageModel.TenantId);
            Assert.Equal("name", page.PageModel.Name);
            Assert.Equal("identifier", page.PageModel.Identifier);
            Assert.Equal("name", page.Name);
        }

        [Fact]
        public async Task OnGetAsync_GivenPageModelIsNotNull_ExpectPageModelNotToBeOverridenAndPageResultReturned()
        {
            var tenantQueryService = new Mock<ITenantQueryService>();
            tenantQueryService.Setup(x => x.GetTenantMetadataById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(new TenantMetadata(
                    TestVariables.TenantId,
                    "identifier",
                    "name",
                    "connection-string",
                    null)));
            var mediator = new Mock<IMediator>();

            var page = new EditTenant(tenantQueryService.Object, mediator.Object)
            {
                PageModel = new EditTenant.Model
                {
                    TenantId = TestVariables.TenantId,
                    Identifier = "identifier-1",
                    Name = "name-1",
                },
            };
            var tempDataDictionary = new Mock<ITempDataDictionary>();
            page.TempData = tempDataDictionary.Object;

            Assert.IsAssignableFrom<PageResult>(await page.OnGetAsync());
            Assert.Equal(TestVariables.TenantId, page.PageModel.TenantId);
            Assert.Equal("name-1", page.PageModel.Name);
            Assert.Equal("identifier-1", page.PageModel.Identifier);
            Assert.Equal("name", page.Name);
        }

        [Fact]
        public async Task OnPostAsync_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var tenantQueryService = new Mock<ITenantQueryService>();
            var mediator = new Mock<IMediator>();

            var page = new EditTenant(tenantQueryService.Object, mediator.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_GivenCommandExecutesSuccessfully_ExpectRedirectToPageResultAndSuccessfulPrgState()
        {
            var tenantQueryService = new Mock<ITenantQueryService>();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UpdateTenantCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ResultWithError.Ok<ErrorData>());

            var page = new EditTenant(tenantQueryService.Object, mediator.Object)
            {
                PageModel = new EditTenant.Model(),
            };

            var result = await page.OnPostAsync();
            var pageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Success, page.PrgState);
            Assert.Single(page.PageNotifications, x => x.PageNotificationType == NotificationPageModel.PageNotification.Success);
            Assert.Equal(PageLocations.TenantView, pageResult.PageName);
        }

        [Fact]
        public async Task OnPostAsync_GivenCommandFailsToExecute_ExpectRedirectToPageResultAndFailedPrgState()
        {
            var tenantQueryService = new Mock<ITenantQueryService>();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UpdateTenantCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));

            var page = new EditTenant(tenantQueryService.Object, mediator.Object)
            {
                PageModel = new EditTenant.Model(),
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
                var cmd = new EditTenant.Model { TenantId = TestVariables.TenantId, Name = "name", Identifier = "identifier" };
                var validator = new EditTenant.Validator();
                var result = validator.Validate(cmd);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenTenantIdIsEmpty_ExpectValidationFailure()
            {
                var cmd = new EditTenant.Model { TenantId = Guid.Empty, Name = "name", Identifier = "identifier" };
                var validator = new EditTenant.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "TenantId");
            }

            [Fact]
            public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
            {
                var cmd = new EditTenant.Model { TenantId = TestVariables.TenantId, Name = string.Empty, Identifier = "identifier" };
                var validator = new EditTenant.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Name");
            }

            [Fact]
            public void Validate_GivenNameIsNull_ExpectValidationFailure()
            {
                var cmd = new EditTenant.Model { TenantId = TestVariables.TenantId, Name = null, Identifier = "identifier" };
                var validator = new EditTenant.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Name");
            }

            [Fact]
            public void Validate_GivenIdentifierIsEmpty_ExpectValidationFailure()
            {
                var cmd = new EditTenant.Model { TenantId = TestVariables.TenantId, Name = "name", Identifier = string.Empty };
                var validator = new EditTenant.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Identifier");
            }

            [Fact]
            public void Validate_GiveIdentifierIsNull_ExpectValidationFailure()
            {
                var cmd = new EditTenant.Model { TenantId = TestVariables.TenantId,  Name = "name", Identifier = null };
                var validator = new EditTenant.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Identifier");
            }
        }
    }
}