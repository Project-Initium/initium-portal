// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Initium.Portal.Queries.Management.Contracts;
using Initium.Portal.Queries.Models.Tenant;
using Initium.Portal.Web.Infrastructure.PageModels;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Management.Pages.App.Tenants
{
    public class ViewTenant : NotificationPageModel
    {
        private readonly ITenantQueryService _tenantQueryService;

        public ViewTenant(ITenantQueryService tenantQueryService)
        {
            this._tenantQueryService = tenantQueryService ?? throw new ArgumentNullException(nameof(tenantQueryService));
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public TenantMetadata Tenant { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var tenantMaybe = await this._tenantQueryService.GetTenantMetadataById(this.Id);
            if (tenantMaybe.HasNoValue)
            {
                return this.NotFound();
            }

            this.Tenant = tenantMaybe.Value;

            return this.Page();
        }
    }
}