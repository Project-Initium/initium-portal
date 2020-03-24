// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stance.Domain.Commands.UserAggregate;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.App.Profile
{
    [Authorize]
    public class Password : PrgPageModel<Password.Model>
    {
        private readonly IMediator _mediator;

        public Password(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result = await this._mediator.Send(new ChangePasswordCommand(
                this.PageModel.OldPassword, this.PageModel.NewPassword));

            this.PrgState = result.IsSuccess ? PrgState.Success : PrgState.Failed;

            return this.RedirectToPage();
        }

        public class Model
        {
            public string OldPassword { get; set; }

            public string NewPassword { get; set; }

            public string ConfirmPassword { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.OldPassword).NotEmpty();
                this.RuleFor(x => x.NewPassword).NotEmpty();
                this.RuleFor(x => x.ConfirmPassword).Equal(x => x.NewPassword);
            }
        }
    }
}