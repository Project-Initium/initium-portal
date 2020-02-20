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
    public class RequestPasswordReset : PrgPageModel<RequestPasswordReset.Model>
    {
        private readonly IMediator _mediator;

        public RequestPasswordReset(IMediator mediator)
        {
            this._mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            await this._mediator.Send(new RequestPasswordResetCommand(this.PageModel.EmailAddress));

            this.PrgState = PrgState.Success;
            return this.RedirectToPage();
        }

        public class Model
        {
            public string EmailAddress { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.EmailAddress).EmailAddress().NotEmpty();
            }
        }
    }
}