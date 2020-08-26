// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Domain.Commands.RoleAggregate;
using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.PageModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Pages.App.UserManagement.Roles
{
    [ResourceBasedAuthorize("role-create")]
    public class CreateRole : PrgPageModel<CreateRole.Model>
    {
        private readonly IMediator _mediator;

        public CreateRole(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public void OnGet()
        {
            this.PageModel ??= new Model();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result = await this._mediator.Send(new CreateRoleCommand(this.PageModel.Name, this.PageModel.Roles));

            if (result.IsSuccess)
            {
                this.PrgState = PrgState.Success;
                this.AddPageNotification("The role was created successfully", PageNotification.Success);
                return this.RedirectToPage(PageLocations.RoleView, new { id = result.Value.RoleId });
            }

            this.AddPageNotification("There was an issue creating the role.", PageNotification.Error);
            this.PrgState = PrgState.Failed;
            return this.RedirectToPage();
        }

        public class Model
        {
            public Model()
            {
                this.Roles = new List<Guid>();
            }

            public string Name { get; set; }

            public List<Guid> Roles { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.Name).NotEmpty();
            }
        }
    }
}