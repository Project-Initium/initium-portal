// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Core.Constants;
using Initium.Portal.Domain.Commands.SystemAlertAggregate;
using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.PageModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Pages.App.SystemAlerts
{
    [ResourceBasedAuthorize("system-alert-create")]
    public class CreateSystemAlert : PrgPageModel<CreateSystemAlert.Model>
    {
        private readonly IMediator _mediator;

        public CreateSystemAlert(IMediator mediator)
        {
            this._mediator = mediator;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result = await this._mediator.Send(new CreateNewSystemAlertCommand(
                this.PageModel.Name,
                this.PageModel.Message,
                this.PageModel.Type,
                this.PageModel.WhenToShow,
                this.PageModel.WhenToHide));

            if (result.IsSuccess)
            {
                this.PrgState = PrgState.Success;
                this.AddPageNotification("The system alert was created successfully", PageNotification.Success);
                return this.RedirectToPage(CorePageLocations.SystemAlertView, new { id = result.Value.SystemAlertId });
            }

            this.AddPageNotification("There was an issue creating the system alert.", PageNotification.Error);
            this.PrgState = PrgState.Failed;
            return this.RedirectToPage();
        }

        public class Model
        {
            public string Name { get; set; }

            public string Message { get; set; }

            public SystemAlertType Type { get; set; }

            public DateTime? WhenToShow { get; set; }

            public DateTime? WhenToHide { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.Name)
                    .NotEmpty();

                this.RuleFor(x => x.Message)
                    .NotEmpty();
            }
        }
    }
}