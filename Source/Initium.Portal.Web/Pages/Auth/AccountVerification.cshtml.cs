// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Core.Settings;
using Initium.Portal.Core.Validation;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.Infrastructure.PageModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Initium.Portal.Web.Pages.Auth
{
    public class AccountVerification : PrgPageModel<AccountVerification.Model>
    {
        private readonly IMediator _mediator;

        public AccountVerification(IMediator mediator)
        {
            this._mediator = mediator;
        }

        [BindProperty(SupportsGet = true)]
        public string Token { get; set; }

        public void OnGet()
        {
            this.PageModel ??= new Model
            {
                Token = this.Token,
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage(new { this.PageModel.Token });
            }

            var result =
                await this._mediator.Send(new VerifyAccountAndSetPasswordCommand(this.PageModel.Token, this.PageModel.Password));

            if (result.IsSuccess)
            {
                this.PrgState = PrgState.Success;
                return this.RedirectToPage();
            }

            this.AddPageNotification("There was an verifying your account. Please try again.", PageNotification.Error);
            this.PrgState = PrgState.Failed;

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
                this.RuleFor(x => x.Token).NotEmpty();
                this.RuleFor(x => x.Password)
                    .NotEmpty().WithMessage("Please enter your new password.")
                    .SetValidator(new PasswordValidator(securitySettings.Value));
                this.RuleFor(x => x.PasswordConfirmation).Equal(x => x.Password).WithMessage("Please confirm your new password.");
            }
        }
    }
}