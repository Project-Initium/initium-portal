// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.Auth
{
    public class PasswordReset : PrgPageModel<PasswordReset.Model>
    {
        private readonly IMediator _mediator;

        public PasswordReset(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        public void OnGet()
        {
            if (this.PageModel == null)
            {
                this.PageModel = new Model
                {
                    Token = this.Token,
                };
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result =
                await this._mediator.Send(new PasswordResetCommand(this.PageModel.Token, this.PageModel.Password));

            this.PrgState = result.IsFailure ? PrgState.Failed : PrgState.Success;

            return this.RedirectToPage();
        }

        public class Model
        {
            public string Token { get; set; }

            public string Password { get; set; }

            public string PasswordConfirmation { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.Token).NotEmpty();
                this.RuleFor(x => x.Password).NotEmpty();
                this.RuleFor(x => x.PasswordConfirmation).Equal(x => x.Password);
            }
        }
    }
}