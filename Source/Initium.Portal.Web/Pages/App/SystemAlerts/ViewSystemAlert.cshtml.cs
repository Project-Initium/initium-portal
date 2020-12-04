// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models.Messaging;
using Initium.Portal.Web.Infrastructure.PageModels;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Pages.App.SystemAlerts
{
    public class ViewSystemAlert : NotificationPageModel
    {
        private readonly ISystemAlertQueryService _systemAlertQueryService;

        public ViewSystemAlert(ISystemAlertQueryService systemAlertQueryService)
        {
            this._systemAlertQueryService = systemAlertQueryService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public DetailedSystemAlert SystemAlert { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var systemAlertMaybe = await this._systemAlertQueryService.GetDetailedSystemAlertById(this.Id);
            if (systemAlertMaybe.HasNoValue)
            {
                return this.NotFound();
            }

            this.SystemAlert = systemAlertMaybe.Value;

            return this.Page();
        }
    }
}