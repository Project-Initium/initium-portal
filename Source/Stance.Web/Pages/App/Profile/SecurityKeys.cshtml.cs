// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Stance.Domain.Commands.UserAggregate;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static.Models.User;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.App.Profile
{
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
            if (devices.HasValue)
            {
                this.DeviceInfos = devices.Value;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await this._mediator.Send(new RevokeAuthenticatorDeviceCommand(this.PageModel.DeviceId));
            return this.RedirectToPage();
        }

        public class Model
        {
            public Guid DeviceId { get; set; }
        }
    }
}