// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using FluentValidation;
using Initium.Portal.Core.Constants;
using Initium.Portal.Domain.Commands.SystemAlertAggregate;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Web.Infrastructure.Constants;
using Initium.Portal.Web.Infrastructure.PageModels;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Pages.App.SystemAlerts
{
    public class EditSystemAlert : PrgPageModel<EditSystemAlert.Model>
    {
        private readonly IMediator _mediator;
        private readonly ISystemAlertQueryService _systemAlertQueryService;

        public EditSystemAlert(IMediator mediator, ISystemAlertQueryService systemAlertQueryService)
        {
            this._mediator = mediator;
            this._systemAlertQueryService = systemAlertQueryService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public string Name { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var systemAlertMaybe = await this._systemAlertQueryService.GetDetailedSystemAlertById(this.Id);
            if (systemAlertMaybe.HasNoValue)
            {
                return this.NotFound();
            }

            var systemAlert = systemAlertMaybe.Value;
            this.PageModel ??= new Model
            {
                SystemAlertId = systemAlert.SystemAlertId,
                Message = systemAlert.Message,
                Name = systemAlert.Name,
                Type = systemAlert.Type,
                WhenToShow = systemAlert.WhenToShow,
                WhenToHide = systemAlert.WhenToHide,
            };

            this.Name = systemAlert.Name;

            return this.Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!this.ModelState.IsValid)
            {
                return this.RedirectToPage();
            }

            var result = await this._mediator.Send(new UpdateSystemAlertCommand(
                this.PageModel.SystemAlertId,
                this.PageModel.Name,
                this.PageModel.Message,
                this.PageModel.Type,
                this.PageModel.WhenToShow,
                this.PageModel.WhenToHide));

            if (result.IsSuccess)
            {
                this.PrgState = PrgState.Success;
                this.AddPageNotification("The system alert was updated successfully", PageNotification.Success);
                return this.RedirectToPage(CorePageLocations.SystemAlertView, new { id = this.PageModel.SystemAlertId });
            }

            this.AddPageNotification("There was an issue updating the system alert.", PageNotification.Error);
            this.PrgState = PrgState.Failed;
            return this.RedirectToPage();
        }

        public class Model
        {
            public Guid SystemAlertId { get; set; }

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
                this.RuleFor(x => x.SystemAlertId)
                    .NotEqual(Guid.Empty);

                this.RuleFor(x => x.Name)
                    .NotEmpty();

                this.RuleFor(x => x.Message)
                    .NotEmpty();
            }
        }
    }
}