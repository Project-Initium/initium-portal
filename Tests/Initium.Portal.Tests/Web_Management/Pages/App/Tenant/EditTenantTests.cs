// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Initium.Portal.Core.Domain;
using Initium.Portal.Queries.Management.Contracts;
using Initium.Portal.Queries.Management.Tenant;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Management.Infrastructure.Constants;
using Initium.Portal.Web.Management.Pages.App.Tenants;
using MaybeMonad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.FeatureManagement;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web_Management.Pages.App.Tenant
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

            var featureManager = new Mock<IFeatureManager>();

            var page = new EditTenant(tenantQueryService.Object, mediator.Object, featureManager.Object);
            var result = await page.OnGetAsync();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnGetAsync_GivenPageModelIsNull_ExpectPageModelToBeSetFromDatabaseAndPageResultReturned()
        {
            var tenantMetadata = new TenantMetadata(
                TestVariables.TenantId,
                "identifier",
                "name",
                null);
            tenantMetadata.SetSystemFeatures("[0]");
            var tenantQueryService = new Mock<ITenantQueryService>();
            tenantQueryService.Setup(x => x.GetTenantMetadataById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(tenantMetadata));
            var mediator = new Mock<IMediator>();

            var featureManager = new Mock<IFeatureManager>();
            featureManager.Setup(x => x.GetFeatureNamesAsync()).Returns(GetTestFeatures);

            var page = new EditTenant(tenantQueryService.Object, mediator.Object, featureManager.Object);
            var tempDataDictionary = new Mock<ITempDataDictionary>();
            page.TempData = tempDataDictionary.Object;

            Assert.IsAssignableFrom<PageResult>(await page.OnGetAsync());
            Assert.Equal(TestVariables.TenantId, page.PageModel.TenantId);
            Assert.Equal("name", page.PageModel.Name);
            Assert.Equal("identifier", page.PageModel.Identifier);
            Assert.Equal(tenantMetadata, page.Tenant);
        }

        [Fact]
        public async Task OnGetAsync_GivenPageModelIsNotNull_ExpectPageModelNotToBeOverridenAndPageResultReturned()
        {
            var tenantMetadata = new TenantMetadata(
                TestVariables.TenantId,
                "identifier",
                "name",
                null);
            var tenantQueryService = new Mock<ITenantQueryService>();
            tenantQueryService.Setup(x => x.GetTenantMetadataById(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Maybe.From(tenantMetadata));
            var mediator = new Mock<IMediator>();

            var featureManager = new Mock<IFeatureManager>();
            featureManager.Setup(x => x.GetFeatureNamesAsync()).Returns(GetTestFeatures);

            var page = new EditTenant(tenantQueryService.Object, mediator.Object, featureManager.Object)
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
            Assert.Equal(tenantMetadata, page.Tenant);
        }

        [Fact]
        public async Task OnPostAsync_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var tenantQueryService = new Mock<ITenantQueryService>();
            var mediator = new Mock<IMediator>();

            var featureManager = new Mock<IFeatureManager>();

            var page = new EditTenant(tenantQueryService.Object, mediator.Object, featureManager.Object);
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

            var featureManager = new Mock<IFeatureManager>();

            var page = new EditTenant(tenantQueryService.Object, mediator.Object, featureManager.Object)
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

            var featureManager = new Mock<IFeatureManager>();

            var page = new EditTenant(tenantQueryService.Object, mediator.Object, featureManager.Object)
            {
                PageModel = new EditTenant.Model(),
            };

            var result = await page.OnPostAsync();
            var pageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Failed, page.PrgState);
            Assert.Single(page.PageNotifications, x => x.PageNotificationType == NotificationPageModel.PageNotification.Error);
            Assert.Null(pageResult.PageName);
        }

        private static async IAsyncEnumerable<string> GetTestFeatures()
        {
            yield return "feature-1";
            yield return "feature-2";

            await Task.CompletedTask; // to make the compiler warning go away
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