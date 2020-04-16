// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stance.Domain.Commands.UserAggregate;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static.Models.User;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.App.Profile
{
    [Authorize]
    public class SecurityKeys : PrgPageModel<SecurityKeys.Model>
    {
        private readonly IMediator _mediator;
        private readonly IUserQueries _userQueries;

        public SecurityKeys(IUserQueries userQueries, IMediator mediator)
        {
            this._userQueries = userQueries;
            this._mediator = mediator;
        }

        public List<DeviceInfo> DeviceInfos { get; set; }

        public async Task OnGet()
        {
            var devices = await this._userQueries.GetDeviceInfoForCurrentUser();
            this.DeviceInfos = devices.HasValue ? devices.Value : new List<DeviceInfo>();
        }

        // public async Task<IActionResult> OnPostAsync()
        // {
        //     if (!this.ModelState.IsValid)
        //     {
        //         return this.RedirectToPage();
        //     }
        //
        //     var result = await this._mediator.Send(new RevokeAuthenticatorDeviceCommand(this.PageModel.DeviceId));
        //
        //     if (result.IsSuccess)
        //     {
        //         this.PrgState = PrgState.Success;
        //         this.AddPageNotification("The device was successfully revoked.", PageNotification.Success);
        //     }
        //     else
        //     {
        //         this.PrgState = PrgState.Failed;
        //         this.AddPageNotification("There was an issue revoking the device.", PageNotification.Error);
        //     }
        //
        //     return this.RedirectToPage();
        // }

        public class Model
        {
            public string Name { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.Name)
                    .NotEmpty()
                    .WithMessage("Please enter a name.");
            }
        }
    }
}