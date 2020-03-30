// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Stance.Domain.Commands.UserAggregate;
using Stance.Queries.Contracts.Static;
using Stance.Web.Infrastructure.Attributes;
using Stance.Web.Infrastructure.Constants;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.App.UserManagement.Users
{
    [ResourceBasedAuthorize("user-create")]
    public class CreateUser : PrgPageModel<CreateUser.Model>
    {
        private readonly IMediator _mediator;
        private readonly IRoleQueries _roleQueries;

        public CreateUser(IMediator mediator, IRoleQueries roleQueries)
        {
            this._mediator = mediator;
            this._roleQueries = roleQueries;
        }

        public List<SelectListItem> AvailableRoles { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            this.AvailableRoles = new List<SelectListItem>();
            var roles = await this._roleQueries.GetSimpleRoles();
            if (roles.HasValue)
            {
                this.AvailableRoles.AddRange(roles.Value.Select(x => new SelectListItem(x.Name, x.Id.ToString())));
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result = await this._mediator.Send(new CreateUserCommand(this.PageModel.EmailAddress, this.PageModel.FirstName,
                this.PageModel.LastName, this.PageModel.IsLockable, this.PageModel.IsAdmin, this.PageModel.Roles));

            if (result.IsSuccess)
            {
                this.PrgState = PrgState.Success;
                this.AddPageNotification("User Creation", "The user was created successfully", PageNotification.Success);
                return this.RedirectToPage(PageLocations.UserView, new { id = result.Value.UserId });
            }

            this.AddPageNotification("User Creation", "There was an issue creating the user.", PageNotification.Error);
            this.PrgState = PrgState.Failed;
            return this.RedirectToPage();
        }

        public class Model
        {
            public Model()
            {
                this.Roles = new List<Guid>();
            }

            [Display(Name = "Email Address")]
            public string EmailAddress { get; set; }

            [Display(Name = "Is Lockable")]
            public bool IsLockable { get; set; }

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Display(Name = "Is Admin")]
            public bool IsAdmin { get; set; }

            public List<Guid> Roles { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.EmailAddress)
                    .NotEmpty()
                    .EmailAddress();
            }
        }
    }
}