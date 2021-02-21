// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Domain.Commands.UserAggregate;
using Initium.Portal.Web.Infrastructure.PageModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Pages.Auth
{
    public class RequestPasswordReset : PrgPageModel<RequestPasswordReset.Model>
    {
        private readonly IMediator _mediator;

        public RequestPasswordReset(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            await this._mediator.Send(new RequestPasswordResetCommand(this.PageModel.EmailAddress));

            this.PrgState = PrgState.Success;
            this.AddPageNotification("You will shortly receive an email with a reset link.", PageNotification.Info);
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