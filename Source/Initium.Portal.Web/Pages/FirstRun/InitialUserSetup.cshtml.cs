// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.PageModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Pages.FirstRun
{
    public class InitialUserSetup : PrgPageModel<InitialUserSetup.Model>
    {
        private readonly IUserQueryService _userQueryService;
        private readonly IMediator _mediator;

        public InitialUserSetup(IUserQueryService userQueryService, IMediator mediator)
        {
            this._userQueryService = userQueryService;
            this._mediator = mediator;
        }

        public async Task<IActionResult> OnGet()
        {
            var check = await this._userQueryService.CheckForPresenceOfAnyUser();
            if (check.IsPresent)
            {
                return this.NotFound();
            }

            return this.Page();
        }

        public async Task<IActionResult> OnPost()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result =
                await this._mediator.Send(new CreateUserCommand(
                    this.PageModel.EmailAddress, this.PageModel.FirstName, this.PageModel.LastName, false, true, new List<Guid>()));

            if (!result.IsFailure)
            {
                return this.RedirectToPage(CorePageLocations.FirstRunSetupCompleted);
            }

            this.PrgState = PrgState.Failed;
            return this.RedirectToPage();
        }

        public class Model
        {
            [Display(Name = "Email Address")]
            public string EmailAddress { get; set; }

            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Display(Name = "Last Name")]
            public string LastName { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.EmailAddress)
                    .NotEmpty().WithMessage("Please enter your email address")
                    .EmailAddress().WithMessage("The email address doesn't look valid");
                this.RuleFor(x => x.FirstName)
                    .NotEmpty().WithMessage("Please enter your first name");
                this.RuleFor(x => x.LastName)
                    .NotEmpty().WithMessage("Please enter your last name");
            }
        }
    }
}