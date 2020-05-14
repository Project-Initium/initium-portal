// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Initium.Portal.Core.Domain;
using Initium.Portal.Domain.Commands.RoleAggregate;
using Initium.Portal.Queries.Contracts.Static;
using Initium.Portal.Queries.Static.Models.Role;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.PageModels;
using Initium.Portal.Web.Pages.App.UserManagement.Roles;
using MaybeMonad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using ResultMonad;
using Xunit;

namespace Initium.Portal.Tests.Web.Pages.App.UserManagement.Roles
{
    public class EditRoleTests
    {
        [Fact]
        public async Task
            OnGetAsync_GivenRoleIsInSystemAndPageModelIsNull_ExpectPageModelNotToBeUpdatedAndPageResultReturned()
        {
            var mediator = new Mock<IMediator>();
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.GetDetailsOfRoleById(It.IsAny<Guid>()))
                .ReturnsAsync(() => Maybe.From(new DetailedRoleModel(
                    TestVariables.RoleId,
                    "name",
                    new List<Guid> { TestVariables.ResourceId })));

            var page = new EditRole(mediator.Object, roleQueries.Object);

            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal(TestVariables.RoleId, page.PageModel.RoleId);
            Assert.Equal("name", page.PageModel.Name);
            Assert.Equal("name", page.Name);
            Assert.Single(page.PageModel.Resources);
        }

        [Fact]
        public async Task OnGetAsync_GivenRoleIsInSystemAndPageModelIsNotNull_ExpectPageModelSetAndPageResultReturned()
        {
            var mediator = new Mock<IMediator>();
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.GetDetailsOfRoleById(It.IsAny<Guid>()))
                .ReturnsAsync(() => Maybe.From(new DetailedRoleModel(
                    TestVariables.RoleId,
                    "old-name",
                    new List<Guid> { TestVariables.ResourceId })));

            var page = new EditRole(mediator.Object, roleQueries.Object)
            {
                PageModel = new EditRole.Model
                {
                    RoleId = TestVariables.RoleId,
                    Name = "name",
                    Resources = new List<Guid> { TestVariables.ResourceId },
                },
            };

            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal(TestVariables.RoleId, page.PageModel.RoleId);
            Assert.Equal("name", page.PageModel.Name);
            Assert.Equal("old-name", page.Name);
            Assert.Single(page.PageModel.Resources);
        }

        [Fact]
        public async Task OnGetAsync_GivenRoleIsNotInSystem_ExpectNotFoundResultReturned()
        {
            var mediator = new Mock<IMediator>();
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.GetDetailsOfRoleById(It.IsAny<Guid>()))
                .ReturnsAsync(() => Maybe<DetailedRoleModel>.Nothing);

            var page = new EditRole(mediator.Object, roleQueries.Object);

            var result = await page.OnGetAsync();
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_GivenInvalidModelState_ExpectRedirectToPageResultToSelfAndNoCommandExecuted()
        {
            var mediator = new Mock<IMediator>();
            var roleQueries = new Mock<IRoleQueries>();
            var page = new EditRole(mediator.Object, roleQueries.Object);
            page.ModelState.AddModelError("Error", "Error");

            var result = await page.OnPostAsync();
            mediator.Verify(x => x.Send(It.IsAny<UpdateRoleCommand>(), It.IsAny<CancellationToken>()), Times.Never);
            Assert.IsType<RedirectToPageResult>(result);
        }

        [Fact]
        public async Task OnPostAsync_GivenValidModelStateAndFailedResult_ExpectRedirectToPageResultToSelfAndPrgSet()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UpdateRoleCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(() => ResultWithError.Fail(new ErrorData(ErrorCodes.SavingChanges)));
            var roleQueries = new Mock<IRoleQueries>();
            var page = new EditRole(mediator.Object, roleQueries.Object) { PageModel = new EditRole.Model() };

            var result = await page.OnPostAsync();
            Assert.Equal(PrgState.Failed, page.PrgState);
            var actualResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.True(string.IsNullOrWhiteSpace(actualResult.PageName));
        }

        [Fact]
        public async Task
            OnPostAsync_GivenValidModelStateAndSuccessfulResult_ExpectRedirectToPageResultToViewPageAndIdSet()
        {
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UpdateRoleCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultWithError.Ok<ErrorData>());
            var roleQueries = new Mock<IRoleQueries>();
            var page = new EditRole(mediator.Object, roleQueries.Object)
                { PageModel = new EditRole.Model { RoleId = TestVariables.RoleId } };

            var result = await page.OnPostAsync();
            var actualResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PageLocations.RoleView, actualResult.PageName);
            Assert.Equal(TestVariables.RoleId, actualResult.RouteValues["id"]);
        }

        public class ValidatorTests
        {
            [Fact]
            public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
            {
                var model = new EditRole.Model { Name = "name", RoleId = TestVariables.RoleId };
                var validator = new EditRole.Validator();
                var result = validator.Validate(model);
                Assert.True(result.IsValid);
            }

            [Fact]
            public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
            {
                var model = new EditRole.Model { Name = string.Empty, RoleId = TestVariables.RoleId };
                var validator = new EditRole.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "Name");
            }

            [Fact]
            public void Validate_GivenNameIsNull_ExpectValidationFailure()
            {
                var model = new EditRole.Model { Name = null, RoleId = TestVariables.RoleId };
                var validator = new EditRole.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "Name");
            }

            [Fact]
            public void Validate_GivenRoleIdIsEmpty_ExpectValidationFailure()
            {
                var model = new EditRole.Model { Name = "name", RoleId = Guid.Empty };
                var validator = new EditRole.Validator();
                var result = validator.Validate(model);
                Assert.False(result.IsValid);
                Assert.Contains(result.Errors, x => x.PropertyName == "RoleId");
            }
        }
    }
}