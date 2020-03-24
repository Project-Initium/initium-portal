// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.Auth
{
    public class RequestAccountVerification : PrgPageModel<RequestAccountVerification.Model>
    {
        private readonly IMediator _mediator;

        public RequestAccountVerification(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            await this._mediator.Send(new RequestAccountVerificationCommand(this.PageModel.EmailAddress));

            this.AddPageNotification("Account verification request", "You will shortly receive an email with a verification link.", PageNotification.Info);
            return this.RedirectToPage();
        }

        public class Model
        {
            [Display(Name = "Email Address")]
            public string EmailAddress { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.EmailAddress)
                    .NotEmpty().WithMessage("Please enter your email address.")
                    .EmailAddress().WithMessage("The email address is not valid");
            }
        }
    }
}