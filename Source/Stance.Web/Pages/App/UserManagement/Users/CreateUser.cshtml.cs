// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.Constants;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.App.UserManagement.Users
{
    public class CreateUser : PrgPageModel<CreateUser.Model>
    {
        private readonly IMediator _mediator;

        public CreateUser(IMediator mediator)
        {
            this._mediator = mediator;
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
                return this.RedirectToPage(PageLocations.UserListing);
            }

            this.PrgState = PrgState.Failed;
            return this.RedirectToPage();
        }

        public class Model
        {
            [Display(Name = "Email Address")]
            public string EmailAddress { get; set; }

            [Display(Name = "Is Lockable")]
            public bool IsLockable { get; set; }

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }

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