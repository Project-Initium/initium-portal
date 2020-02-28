// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stance.Domain.Commands.RoleAggregate;
using Stance.Queries.Contracts;
using Stance.Web.Infrastructure.Constants;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.App.UserManagement.Roles
{
    public class EditRole : PrgPageModel<EditRole.Model>
    {
        private readonly IMediator _mediator;
        private readonly IRoleQueries _roleQueries;

        public EditRole(IMediator mediator, IRoleQueries roleQueries)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._roleQueries = roleQueries ?? throw new ArgumentNullException(nameof(roleQueries));
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public string Name { get; private set; }

        public async Task<IActionResult> OnGet()
        {
            var roleMaybe = await this._roleQueries.GetDetailsOfRoleById(this.Id);
            if (roleMaybe.HasNoValue)
            {
                return this.NotFound();
            }

            var role = roleMaybe.Value;
            if (this.PageModel == null)
            {
                this.PageModel = new Model
                {
                    Id = role.Id,
                    Name = role.Name,
                    Resources = role.Resources.ToList(),
                };
            }

            this.Name = role.Name;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result = await this._mediator.Send(new UpdateRoleCommand(this.PageModel.Id, this.PageModel.Name,
                this.PageModel.Resources));

            if (result.IsSuccess)
            {
                return this.RedirectToPage(PageLocations.RoleView, new { id = this.PageModel.Id });
            }

            this.PrgState = PrgState.Failed;
            return this.RedirectToPage();
        }

        public class Model
        {
            public Guid Id { get; set; }

            public string Name { get; set; }

            public List<Guid> Resources { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.Name)
                    .NotEmpty();
                this.RuleFor(x => x.Id)
                    .NotEqual(Guid.Empty);
            }
        }
    }
}