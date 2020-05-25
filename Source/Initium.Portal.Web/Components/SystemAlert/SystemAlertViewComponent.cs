// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Initium.Portal.Queries.Contracts.Static;
using Initium.Portal.Queries.Static.Models.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Components.SystemAlert
{
    public class SystemAlertViewComponent : ViewComponent
    {
        private readonly IMessagingQueries _messagingQueries;

        public SystemAlertViewComponent(IMessagingQueries messagingQueries)
        {
            this._messagingQueries = messagingQueries ?? throw new ArgumentNullException(nameof(messagingQueries));
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var data = await this._messagingQueries.GetActiveSystemAlerts();

            return this.View(data.HasValue ? data.Value : new List<ActiveSystemAlert>());
        }
    }
}