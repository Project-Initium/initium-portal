// Copyright (c) DeviousCreation. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Stance.Queries.Contracts.Static;
using Stance.Queries.Static.Models.User;
using Stance.Web.Infrastructure.PageModels;

namespace Stance.Web.Pages.App.Profile
{
    [Authorize]
    public class SecurityKeys : PrgPageModel<SecurityKeys.Model>
    {
        private readonly IUserQueries _userQueries;

        public SecurityKeys(IUserQueries userQueries)
        {
            this._userQueries = userQueries;
        }

        public List<DeviceInfo> DeviceInfos { get; set; }

        public async Task OnGet()
        {
            var devices = await this._userQueries.GetDeviceInfoForCurrentUser();
            this.DeviceInfos = devices.HasValue ? devices.Value : new List<DeviceInfo>();
        }

        public class Model
        {
            public string Name { get; set; }

            public string Password { get; set; }
        }

        public class Validator : AbstractValidator<Model>
        {
            public Validator()
            {
                this.RuleFor(x => x.Name)
                    .NotEmpty()
                    .WithMessage("Please enter a name.");

                this.RuleFor(x => x.Password)
                    .NotEmpty()
                    .WithMessage("Please enter your password.");
            }
        }
    }
}