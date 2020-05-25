// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Initium.Portal.Queries.Contracts.Static;
using Initium.Portal.Queries.Static.Models.Messaging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Initium.Portal.Web.Pages.App.SystemAlerts
{
    public class ViewSystemAlert : PageModel
    {
        private readonly IMessagingQueries _messagingQueries;

        public ViewSystemAlert(IMessagingQueries messagingQueries)
        {
            this._messagingQueries = messagingQueries;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public DetailedSystemAlert SystemAlert { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var systemAlertMaybe = await this._messagingQueries.GetDetailedSystemAlertById(this.Id);
            if (systemAlertMaybe.HasNoValue)
            {
                return this.NotFound();
            }

            this.SystemAlert = systemAlertMaybe.Value;

            return this.Page();
        }
    }
}