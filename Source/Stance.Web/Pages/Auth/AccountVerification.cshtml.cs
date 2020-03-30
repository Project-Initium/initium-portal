// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stance.Core.Settings;
using Stance.Core.Validation;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.Auth
{
    public class AccountVerification : PrgPageModel<AccountVerification.Model>
    {
        private readonly IMediator _mediator;

        public AccountVerification(IMediator mediator)
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
                return this.RedirectToPage(new { this.PageModel.Token });
            }

            var result =
                await this._mediator.Send(new VerifyAccountAndSetPasswordCommand(this.PageModel.Token, this.PageModel.Password));

            this.PrgState = result.IsFailure ? PrgState.Failed : PrgState.Success;

            return this.RedirectToPage(new { this.PageModel.Token });
        }

        public class Model
        {
            public string Token { get; set; }

            public string Password { get; set; }

            [Display(Name = "Password Confirmation")]
            public string PasswordConfirmation { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator(IOptions<SecuritySettings> securitySettings)
            {
                if (securitySettings == null)
                {
                    throw new ArgumentNullException(nameof(securitySettings));
                }

                this.RuleFor(x => x.Token).NotEmpty();
                this.RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Please enter your new password.")
                    .SetValidator(new PasswordValidator(securitySettings.Value));
                this.RuleFor(x => x.PasswordConfirmation).Equal(x => x.Password).WithMessage("Please confirm your new password.");
            }
        }
    }
}