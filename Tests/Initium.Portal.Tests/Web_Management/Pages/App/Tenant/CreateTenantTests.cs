// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Common.Domain.Commands.TenantAggregate;
using Initium.Portal.Core.Domain;
using Initium.Portal.Core.Settings;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Management.Infrastructure.Constants;
using Initium.Portal.Web.Management.Pages.App.Tenants;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web_Management.Pages.App.Tenant
{
    public class CreateTenantTests
    {
        [Fact]
        public async Task OnPostAsync_GivenInvalidModelState_ExpectRedirectToPageResult()
        {
            var mediator = new Mock<IMediator>();
            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            var featureManager = new Mock<IFeatureManager>();

            var page = new CreateTenant(mediator.Object, multiTenantSettings.Object, featureManager.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPostAsync();
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_GivenCommandExecutesSuccessfully_ExpectRedirectToPageResultAndSuccessfulPrgState()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CreateTenantCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ResultWithError.Ok<ErrorData>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantConnectionString = "Server=.,1434;Database=initium;User Id=sa;Password=mIOub5n3nG8LEpaa;",
            });

            var featureManager = new Mock<IFeatureManager>();

            var page = new CreateTenant(mediator.Object, multiTenantSettings.Object, featureManager.Object)
            {
                PageModel = new CreateTenant.Model(),
            };

            var result = await page.OnPostAsync();
            var pageResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PrgState.Success, page.PrgState);
            Assert.Single(page.PageNotifications, x => x.PageNotificationType == NotificationPageModel.PageNotification.Success);
            Assert.Equal(PageLocations.TenantView, pageResult.PageName);
        }

        [Fact]
        public async Task OnPostAsync_GivenMultiTenantTypeIsTableSplit_ExpectOriginalConnectionStringUsed()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CreateTenantCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ResultWithError.Ok<ErrorData>());

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings
            {
                DefaultTenantConnectionString = "Server=.,1434;Database=initium;User Id=sa;Password=mIOub5n3nG8LEpaa;",
            });

            var featureManager = new Mock<IFeatureManager>();

            var page = new CreateTenant(mediator.Object, multiTenantSettings.Object, featureManager.Object)
            {
                PageModel = new CreateTenant.Model(),
            };

            await page.OnPostAsync();
            mediator.Verify(x => x.Send(It.Is<CreateTenantCommand>(y => y.ConnectionString == "Server=.,1434;Database=initium;User Id=sa;Password=mIOub5n3nG8LEpaa;"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task OnPostAsync_GivenCommandFailsToExecute_ExpectRedirectToPageResultAndFailedPrgState()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<CreateTenantCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(
                    ResultWithError.Fail(new ErrorData(ErrorCodes.AuthenticationFailed)));

            var multiTenantSettings = new Mock<IOptions<MultiTenantSettings>>();
            multiTenantSettings.Setup(x => x.Value).Returns(new MultiTenantSettings());

            var featureManager = new Mock<IFeatureManager>();

            var page = new CreateTenant(mediator.Object, multiTenantSettings.Object, featureManager.Object)
            {
                PageModel = new CreateTenant.Model(),
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
                var cmd = new CreateTenant.Model { Name = "name", Identifier = "identifier" };
                var validator = new CreateTenant.Validator();
                var result = validator.Validate(cmd);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
            {
                var cmd = new CreateTenant.Model { Name = string.Empty, Identifier = "identifier" };
                var validator = new CreateTenant.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Name");
            }

            [Fact]
            public void Validate_GivenNameIsNull_ExpectValidationFailure()
            {
                var cmd = new CreateTenant.Model { Name = null, Identifier = "identifier" };
                var validator = new CreateTenant.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Name");
            }

            [Fact]
            public void Validate_GivenIdentifierIsEmpty_ExpectValidationFailure()
            {
                var cmd = new CreateTenant.Model { Name = "name", Identifier = string.Empty };
                var validator = new CreateTenant.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Identifier");
            }

            [Fact]
            public void Validate_GiveIdentifierIsNull_ExpectValidationFailure()
            {
                var cmd = new CreateTenant.Model { Name = "name", Identifier = null };
                var validator = new CreateTenant.Validator();
                var result = validator.Validate(cmd);
                Assert.False(result.IsValid);
                Assert.Contains(
                    result.Errors,
                    failure => failure.PropertyName == "Identifier");
            }
        }
    }
}