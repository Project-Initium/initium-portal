// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stance.Domain.CommandResults.UserAggregate;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.Constants;
using Stance.Web.Infrastructure.Extensions;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.Auth
{
    public class UserAuthentication : PrgPageModel<UserAuthentication.Model>
    {
        private readonly IMediator _mediator;

        public UserAuthentication(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!this.ModelState.IsValid)
            {
                return string.IsNullOrEmpty(this.ReturnUrl)
                    ? this.RedirectToPage()
                    : this.RedirectToPage(new { this.ReturnUrl });
            }

            var result =
                await this._mediator.Send(new AuthenticateUserCommand(
                    this.PageModel.EmailAddress, this.PageModel.Password));

            if (result.IsSuccess)
            {
                if (result.Value.AuthenticationStatus ==
                    BaseAuthenticationProcessCommandResult.AuthenticationState.Completed)
                {
                    await this.HttpContext.SignInUserAsync(new HttpContextExtensions.UserProfile(
                        result.Value.UserId,
                        result.Value.EmailAddress));

                    if (string.IsNullOrEmpty(this.ReturnUrl))
                    {
                        return this.RedirectToPage(PageLocations.AppDashboard);
                    }

                    return this.LocalRedirect(this.ReturnUrl);
                }

                await this.HttpContext.SignInUserPartiallyAsync(
                    new HttpContextExtensions.UserProfile(
                    result.Value.UserId,
                    result.Value.EmailAddress), this.ReturnUrl);

                return this.RedirectToPage(PageLocations.AuthEmailMfa);
            }

            this.PrgState = PrgState.InError;
            return string.IsNullOrEmpty(this.ReturnUrl)
                    ? this.RedirectToPage()
                    : this.RedirectToPage(new { this.ReturnUrl });
        }

        public class Model
        {
            [Display(Name = "Email Address")]
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