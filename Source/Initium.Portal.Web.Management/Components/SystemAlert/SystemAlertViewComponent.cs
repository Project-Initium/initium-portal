// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System.Collections.Generic;
using System.Threading.Tasks;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models.Messaging;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Management.Components.SystemAlert
{
    public class SystemAlertViewComponent : ViewComponent
    {
        private readonly ISystemAlertQueryService _systemAlertQueryService;

        public SystemAlertViewComponent(ISystemAlertQueryService systemAlertQueryService)
        {
            this._systemAlertQueryService = systemAlertQueryService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var data = await this._systemAlertQueryService.GetActiveSystemAlerts();

            return this.View(data.HasValue ? data.Value : new List<ActiveSystemAlert>());
        }
    }
}