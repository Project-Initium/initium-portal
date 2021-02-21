// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Domain.Commands.RoleAggregate;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.PageModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Pages.App.UserManagement.Roles
{
    [ResourceBasedAuthorize("role-edit")]
    public class EditRole : PrgPageModel<EditRole.Model>
    {
        private readonly IMediator _mediator;
        private readonly IRoleQueryService _roleQueryService;

        public EditRole(IMediator mediator, IRoleQueryService roleQueryService)
        {
            this._mediator = mediator;
            this._roleQueryService = roleQueryService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public string Name { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var roleMaybe = await this._roleQueryService.GetDetailsOfRoleById(this.Id);
            if (roleMaybe.HasNoValue)
            {
                return this.NotFound();
            }

            var role = roleMaybe.Value;
            this.PageModel ??= new Model
            {
                RoleId = role.Id,
                Name = role.Name,
                Resources = role.Resources.ToList(),
            };

            this.Name = role.Name;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result = await this._mediator.Send(new UpdateRoleCommand(this.PageModel.RoleId, this.PageModel.Name,
                this.PageModel.Resources));

            if (result.IsSuccess)
            {
                this.PrgState = PrgState.Success;
                this.AddPageNotification("The role was updated successfully", PageNotification.Success);
                return this.RedirectToPage(CorePageLocations.RoleView, new { id = this.PageModel.RoleId });
            }

            this.AddPageNotification("There was an issue updating the role.", PageNotification.Error);
            this.PrgState = PrgState.Failed;
            return this.RedirectToPage();
        }

        public class Model
        {
            public string Name { get; set; }

            public List<Guid> Resources { get; set; }

            public Guid RoleId { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.Name)
                    .NotEmpty();
                this.RuleFor(x => x.RoleId)
                    .NotEqual(Guid.Empty);
            }
        }
    }
}