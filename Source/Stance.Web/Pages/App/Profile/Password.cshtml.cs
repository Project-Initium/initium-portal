// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Stance.Core.Settings;
using Stance.Core.Validation;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.App.Profile
{
    [Authorize]
    public class Password : PrgPageModel<Password.Model>
    {
        private readonly IMediator _mediator;
        private readonly SecuritySettings _securitySettings;

        public Password(IMediator mediator, IOptions<SecuritySettings> securitySettings)
        {
            if (securitySettings == null)
            {
                throw new ArgumentNullException(nameof(securitySettings));
            }

            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            this._securitySettings = securitySettings.Value;
        }

        public int RequiredLength { get; private set; }

        public int RequiredUniqueChars { get; private set; }

        public bool RequireDigit { get; private set; }

        public bool RequireLowercase { get; private set; }

        public bool RequireUppercase { get; private set; }

        public bool RequireNonAlphanumeric { get; private set; }

        public void OnGet()
        {
            this.RequiredLength = this._securitySettings.PasswordRequirements.RequiredLength;
            this.RequiredUniqueChars = this._securitySettings.PasswordRequirements.RequiredUniqueChars;
            this.RequireDigit = this._securitySettings.PasswordRequirements.RequireDigit;
            this.RequireLowercase = this._securitySettings.PasswordRequirements.RequireLowercase;
            this.RequireUppercase = this._securitySettings.PasswordRequirements.RequireUppercase;
            this.RequireNonAlphanumeric = this._securitySettings.PasswordRequirements.RequireNonAlphanumeric;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result = await this._mediator.Send(new ChangePasswordCommand(
                this.PageModel.OldPassword, this.PageModel.NewPassword));

            if (result.IsSuccess)
            {
                this.PrgState = PrgState.Success;
                this.AddPageNotification("Your password has been changed.", PageNotification.Success);
            }
            else
            {
                this.PrgState = PrgState.Failed;
                this.AddPageNotification("There has been an issue changing your password.", PageNotification.Error);
            }

            return this.RedirectToPage();
        }

        public class Model
        {
            [Display(Name = "Old Password")]
            public string OldPassword { get; set; }

            [Display(Name = "New Password")]
            public string NewPassword { get; set; }

            [Display(Name = "Confirm Password")]
            public string ConfirmPassword { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator(IOptions<SecuritySettings> securitySettings)
            {
                this.RuleFor(x => x.OldPassword).NotEmpty();
                this.RuleFor(x => x.NewPassword)
                    .NotEmpty()
                    .SetValidator(new PasswordValidator(securitySettings.Value));

                this.RuleFor(x => x.ConfirmPassword)
                    .Equal(x => x.NewPassword);
            }
        }
    }
}