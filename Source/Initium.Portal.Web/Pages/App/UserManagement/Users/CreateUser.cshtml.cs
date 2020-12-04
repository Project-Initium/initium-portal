// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.PageModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Initium.Portal.Web.Pages.App.UserManagement.Users
{
    [ResourceBasedAuthorize("user-create")]
    public class CreateUser : PrgPageModel<CreateUser.Model>
    {
        private readonly IMediator _mediator;
        private readonly IRoleQueryService _roleQueryService;

        public CreateUser(IMediator mediator, IRoleQueryService roleQueryService)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._roleQueryService = roleQueryService ?? throw new ArgumentNullException(nameof(roleQueryService));
        }

        public List<SelectListItem> AvailableRoles { get; set; }

        public async Task OnGetAsync()
        {
            this.AvailableRoles = new List<SelectListItem>();
            var roles = await this._roleQueryService.GetSimpleRoles();
            if (roles.HasValue)
            {
                this.AvailableRoles.AddRange(roles.Value.Select(x => new SelectListItem(x.Name, x.Id.ToString())));
            }
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
                this.AddPageNotification("The user was created successfully", PageNotification.Success);
                return this.RedirectToPage(CorePageLocations.UserView, new { id = result.Value.UserId });
            }

            this.AddPageNotification("There was an issue creating the user.", PageNotification.Error);
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
                this.RuleFor(x => x.FirstName)
                    .NotEmpty();
                this.RuleFor(x => x.LastName)
                    .NotEmpty();
            }
        }
    }
}