// Copyright (c) Project Initium. All rights reserved.
// Licensed under the MIT License. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using Initium.Portal.Queries.Contracts;
using Initium.Portal.Queries.Models.Role;
using Initium.Portal.Web.Infrastructure.Attributes;
using Initium.Portal.Web.Infrastructure.PageModels;
using Microsoft.AspNetCore.Mvc;

namespace Initium.Portal.Web.Pages.App.UserManagement.Roles
{
    [ResourceBasedAuthorize("role-view")]
    public class ViewRole : NotificationPageModel
    {
        private readonly IRoleQueryService _roleQueryService;

        public ViewRole(IRoleQueryService roleQueryService)
        {
            this._roleQueryService = roleQueryService;
        }

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public string Name { get; private set; }

        public DetailedRoleModel Role { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var roleMaybe = await this._roleQueryService.GetDetailsOfRoleById(this.Id);
            if (roleMaybe.HasNoValue)
            {
                return this.NotFound();
            }

            this.Role = roleMaybe.Value;

            this.Name = this.Role.Name;

            return this.Page();
        }
    }
}