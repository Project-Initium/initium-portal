// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stance.Domain.Commands.UserAggregate;
using Stance.Queries.Contracts;
using Stance.Web.Infrastructure.Constants;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.FirstRun
{
    public class InitialUserSetup : PrgPageModel<InitialUserSetup.Model>
    {
        private readonly IUserQueries _userQueries;
        private readonly IMediator _mediator;

        public InitialUserSetup(IUserQueries userQueries, IMediator mediator)
        {
            this._userQueries = userQueries ?? throw new ArgumentNullException(nameof(userQueries));
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IActionResult> OnGet()
        {
            var check = await this._userQueries.CheckForPresenceOfAnyUser(CancellationToken.None);
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
                await this._mediator.Send(new CreateInitialUserCommand(
                    this.PageModel.EmailAddress, this.PageModel.Password));

            if (result.IsFailure)
            {
                this.PrgState = PrgState.Failed;
                return this.RedirectToPage();
            }

            return this.RedirectToPage(PageLocations.FirstRunSetupCompleted);
        }

        public class Model
        {
            public string EmailAddress { get; set; }

            public string Password { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.EmailAddress).NotEmpty().EmailAddress();
                this.RuleFor(x => x.Password).NotEmpty();
            }
        }
    }
}