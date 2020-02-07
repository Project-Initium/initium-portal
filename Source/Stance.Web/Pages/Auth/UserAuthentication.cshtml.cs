using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.Extensions;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.Auth
{
    public class UserAuthentication : PrgPageModel<UserAuthentication.Model>
    {
        private readonly IMediator _mediator;

        public UserAuthentication(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result =
                await this._mediator.Send(new AuthenticateUserCommand(this.PageModel.EmailAddress, this.PageModel.Password));

            if (result.IsSuccess)
            {
                await this.HttpContext.SignInUser(new HttpContextExtensions.UserProfile(
                    result.Value.UserId,
                    result.Value.EmailAddress));
                // redirect
            }

            this.PrgState = PrgState.InError;
            return this.RedirectToPage();
        }

        public class Model
        {
            [Display(Name = "Email Address")]
            public string EmailAddress { get; set; }

            public string Password { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {

        }
    }
}
