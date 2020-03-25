// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MaybeMonad;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Moq;
using ResultMonad;
using Stance.Core.Domain;
using Stance.Domain.Commands.RoleAggregate;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static.Models.Role;
using Stance.Web.Infrastructure.Constants;
using Stance.Web.Infrastructure.PageModels;
using Stance.Web.Pages.App.UserManagement.Roles;
using Xunit;

namespace Stance.Tests.Web.Pages.App.UserManagement.Roles
{
    public class EditRoleTests
    {
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
        public async Task OnGetAsync_GivenRoleIsInSystemAndPageModelIsNull_ExpectPageModelSetAndPageResultReturned()
        {
            var roleId = Guid.NewGuid();
            var mediator = new Mock<IMediator>();
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.GetDetailsOfRoleById(It.IsAny<Guid>()))
                .ReturnsAsync(() => Maybe.From(new DetailedRoleModel(Guid.Empty, string.Empty, new List<Guid>())));

            var page = new EditRole(mediator.Object, roleQueries.Object)
            {
                PageModel = new EditRole.Model
                {
                    RoleId = roleId,
                    Name = new string('*', 5),
                    Resources = new List<Guid> { Guid.NewGuid() },
                },
            };

            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal(roleId, page.PageModel.RoleId);
            Assert.Equal(new string('*', 5), page.PageModel.Name);
            Assert.Single(page.PageModel.Resources);
        }

        [Fact]
        public async Task OnGetAsync_GivenRoleIsInSystemAndPageModelIsNotNull_ExpectPageModelNotToBeUpdatedAndPageResultReturned()
        {
            var roleId = Guid.NewGuid();
            var mediator = new Mock<IMediator>();
            var roleQueries = new Mock<IRoleQueries>();
            roleQueries.Setup(x => x.GetDetailsOfRoleById(It.IsAny<Guid>()))
                .ReturnsAsync(() => Maybe.From(new DetailedRoleModel(roleId, new string('*', 5), new List<Guid> { Guid.NewGuid() })));

            var page = new EditRole(mediator.Object, roleQueries.Object);

            var result = await page.OnGetAsync();
            Assert.IsType<PageResult>(result);
            Assert.Equal(roleId, page.PageModel.RoleId);
            Assert.Equal(new string('*', 5), page.PageModel.Name);
            Assert.Single(page.PageModel.Resources);
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
            mediator.Setup(x => x.Send(It.IsAny<UpdateRoleCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => ResultWithError.Fail(new ErrorData(ErrorCodes.SavingChanges)));
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
            var roleId = Guid.NewGuid();
            var mediator = new Mock<IMediator>();
            mediator.Setup(x => x.Send(It.IsAny<UpdateRoleCommand>(), It.IsAny<CancellationToken>())).ReturnsAsync(() => ResultWithError.Ok<ErrorData>());
            var roleQueries = new Mock<IRoleQueries>();
            var page = new EditRole(mediator.Object, roleQueries.Object) { PageModel = new EditRole.Model { RoleId = roleId } };

            var result = await page.OnPostAsync();
            var actualResult = Assert.IsType<RedirectToPageResult>(result);
            Assert.Equal(PageLocations.RoleView, actualResult.PageName);
            Assert.Equal(roleId, actualResult.RouteValues["id"]);
        }

        [Fact]
        public void Validate_GivenAllPropertiesAreValid_ExpectValidationSuccess()
        {
            var model = new EditRole.Model { Name = new string('*', 5), RoleId = Guid.NewGuid() };
            var validator = new EditRole.Validator();
            var result = validator.Validate(model);
            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_GivenNameIsEmpty_ExpectValidationFailure()
        {
            var model = new EditRole.Model { Name = string.Empty, RoleId = Guid.NewGuid() };
            var validator = new EditRole.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "Name");
        }

        [Fact]
        public void Validate_GivenNameIsNull_ExpectValidationFailure()
        {
            var model = new EditRole.Model { Name = null, RoleId = Guid.NewGuid() };
            var validator = new EditRole.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "Name");
        }

        [Fact]
        public void Validate_GivenRoleIdIsEmpty_ExpectValidationFailure()
        {
            var model = new EditRole.Model { Name = new string('*', 5), RoleId = Guid.Empty };
            var validator = new EditRole.Validator();
            var result = validator.Validate(model);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, x => x.PropertyName == "RoleId");
        }
    }
}