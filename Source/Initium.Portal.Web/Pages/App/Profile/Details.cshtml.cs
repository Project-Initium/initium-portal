// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Web.Infrastructure.PageModels;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Pages.App.Profile
{
    [Authorize]
    public class Details : PrgPageModel<Details.Model>
    {
        private readonly IMediator _mediator;
        private readonly IUserQueryService _userQueryService;

        public Details(IMediator mediator, IUserQueryService userQueryService)
        {
            this._mediator = mediator;
            this._userQueryService = userQueryService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (this.PageModel == null)
            {
                var profileMaybe = await this._userQueryService.GetProfileForCurrentUser();
                if (profileMaybe.HasNoValue)
                {
                    return this.NotFound();
                }

                this.PageModel = new Model
                {
                    FirstName = profileMaybe.Value.FirstName,
                    LastName = profileMaybe.Value.LastName,
                };
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result =
                await this._mediator.Send(new UpdateProfileCommand(this.PageModel.FirstName, this.PageModel.LastName));

            if (result.IsSuccess)
            {
                this.PrgState = PrgState.Success;
                this.AddPageNotification("Your details have been changed.", PageNotification.Success);
            }
            else
            {
                this.PrgState = PrgState.Failed;
                this.AddPageNotification("There has been an issue changing your details.", PageNotification.Error);
            }

            return this.RedirectToPage();
        }

        public class Model
        {
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage("Please enter your first name");
                this.RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage("Please enter your last name");
            }
        }
    }
}